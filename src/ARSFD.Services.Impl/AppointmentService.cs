using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DATABASE = ARSFD.Database;

namespace ARSFD.Services.Impl
{
	public class AppointmentService: IAppointmentService
	{
		private DATABASE.ApplicationDbContext _context;

		public AppointmentService(
			DATABASE.ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		#region IAppointmentService members

		public async Task Cancel(
			int appointmentId,
			int userId,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.Appointment appointment = await _context
					.Appointments
					.FirstAsync(x => x.Id == appointmentId, cancellationToken);

				if (appointment.CanceledOn != null || appointment.CanceledById != null)
				{
					throw new ServiceException($"Appointment with identifier `{appointmentId}` is already canceled.");
				}
				
				appointment.CanceledById = userId;
				appointment.CanceledOn = DateTime.Now;

				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to cancel appointment with identifier `{appointmentId}`.", ex);
			}
		}

		public async Task<Appointment> Get(
			int appointmentId,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.Appointment appointment = await _context
					.Appointments
					.FirstAsync(x => x.Id == appointmentId, cancellationToken);

				var app = new Appointment
				{
					Id = appointment.Id,
					CanceledById = appointment.CanceledById,
					CanceledOn = appointment.CanceledOn,
					Date = appointment.Date,
					DoctorId = appointment.DoctorId,
					UserId = appointment.UserId,
				};

				return app;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get appointment with identifier `{appointmentId}`.", ex);
			}
		}

		public async Task Create(
			Appointment appointment,
			CancellationToken cancellationToken = default)
		{
			try
			{
				#region Validation

				if (appointment == null)
				{
					throw new ArgumentNullException(nameof(appointment));
				}

				if (appointment.Date == default)
				{
					throw new ArgumentException("Invalid appointment date.");
				}

				#endregion

				var app = new DATABASE.Appointment
				{
					UserId = appointment.UserId,
					DoctorId = appointment.DoctorId,
					Date = appointment.Date,
				};

				await _context.Appointments.AddAsync(app, cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException("Failed to create appointment.", ex);
			}
		}

		public async Task<FindResult<Appointment>> Find(
			FindAppointmentsFilter filter = null,
			FindOptions options = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				IQueryable<DATABASE.Appointment> entities = _context.Appointments;

				entities = FilterEntities(entities, filter);

				// TODO: paging

				DATABASE.Appointment[] items = await entities
					.ToArrayAsync(cancellationToken);

				Appointment[] serviceItems = items
					.Select(x => new Appointment(x))
					.ToArray();

				var result = new FindResult<Appointment>(serviceItems);
				return result;
			}
			catch (Exception ex)
			{
				throw new ServiceException("Failed to find appointments.", ex);
			}
		}

		#endregion

		private IQueryable<DATABASE.Appointment> FilterEntities(
			IQueryable<DATABASE.Appointment> entities,
			FindAppointmentsFilter filter)
		{
			if (filter.IsSet(x => x.DoctorId))
			{
				entities = entities.Where(x => x.DoctorId == filter.DoctorId);
			}

			if (filter.IsSet(x => x.UserId))
			{
				entities = entities.Where(x => x.UserId == filter.UserId);
			}

			if (filter.IsSet(x => x.CanceledById))
			{
				entities = entities.Where(x => x.CanceledById == filter.CanceledById);
			}

			if (filter.IsSet(x => x.Canceled))
			{
				if (filter.Canceled)
				{
					entities = entities.Where(x => x.CanceledOn != null || x.CanceledById != null);
				}
				else
				{
					entities = entities.Where(x => x.CanceledOn == null && x.CanceledById == null);
				}
			}

			if (filter.IsSet(x => x.Date))
			{
				entities = entities.Where(x => x.Date.Date == filter.Date.Date);
			}

			return entities;
		}
	}
}
