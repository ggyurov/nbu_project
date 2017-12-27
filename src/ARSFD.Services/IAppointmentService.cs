using System.Threading;
using System.Threading.Tasks;

namespace ARSFD.Services
{
	public interface IAppointmentService
	{
		Task Create(
			Appointment appointment,
			CancellationToken cancellationToken = default);

		Task Cancel(
			int appointmentId,
			CancellationToken cancellationToken = default);

		Task<Appointment> Get(
			int appointmentId,
			CancellationToken cancellationToken = default);

		Task<FindResult<Appointment>> Find(
			FindAppointmentsFilter filter = null,
			FindOptions options = null,
			CancellationToken cancellationToken = default);
	}
}
