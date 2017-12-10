using System;
using System.ComponentModel.DataAnnotations;

namespace ARSFD.Database
{
	public class Appointment
	{
		public int Id { get; set; }

		[MaxLength(450)]
		public string UserId { get; set; }

		public DateTime Date { get; set; }

		[MaxLength(450)]
		public string DoctorId { get; set; }

		[MaxLength(450)]
		public string CanceledById { get; set; }

		public DateTime? CanceledOn { get; set; }
	}
}
