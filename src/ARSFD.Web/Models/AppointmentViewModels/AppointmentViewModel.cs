using System;

namespace ARSFD.Web.Models.AppointmentViewModels
{
	public class AppointmentViewModel
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public string UserName { get; set; }

		public DateTime Date { get; set; }

		public int DoctorId { get; set; }

		public string DoctorName { get; set; }

		public int? CanceledById { get; set; }

		public DateTime? CanceledOn { get; set; }

		public string CancelUrl { get; set; }
	}
}
