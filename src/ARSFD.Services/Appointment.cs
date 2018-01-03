using System;

namespace ARSFD.Services
{
	public class Appointment
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public DateTime Date { get; set; }

		public int DoctorId { get; set; }

		public int? CanceledById { get; set; }

		public DateTime? CanceledOn { get; set; }

		public Appointment()
		{

		}

		public Appointment(Database.Appointment appointment)
		{
			if (appointment == null)
			{
				throw new ArgumentNullException(nameof(appointment));
			}

			Id = appointment.Id;
			UserId = appointment.UserId;
			Date = appointment.Date;
			DoctorId = appointment.DoctorId;
			CanceledById = appointment.CanceledById;
			CanceledOn = appointment.CanceledOn;
		}
	}
}
