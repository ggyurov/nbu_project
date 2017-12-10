using System;

namespace ARSFD.Services
{
	public class Appointment
	{
		public int Id { get; set; }

		public string UserId { get; set; }

		public DateTime Date { get; set; }

		public string DoctorId { get; set; }

		public string CanceledById { get; set; }

		public DateTime? CanceledOn { get; set; }
	}
}
