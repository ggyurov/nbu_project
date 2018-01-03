using System;

namespace ARSFD.Database
{
	public class Appointment
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public DateTime Date { get; set; }

		public int DoctorId { get; set; }

		public int? CanceledById { get; set; }

		public DateTime? CanceledOn { get; set; }
	}
}
