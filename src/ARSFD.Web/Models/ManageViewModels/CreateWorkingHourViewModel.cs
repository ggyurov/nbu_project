using System;
using System.ComponentModel.DataAnnotations;

namespace ARSFD.Web.Models.ManageViewModels
{
	public class CreateWorkingHourViewModel
	{
		[Display(Name = "Ден")]
		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		public DayOfWeek DayOfWeek { get; set; }

		[Display(Name = "Начало")]
		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		public DateTime StartTime { get; set; }

		[Display(Name = "Край")]
		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		public DateTime EndTime { get; set; }
	}
}
