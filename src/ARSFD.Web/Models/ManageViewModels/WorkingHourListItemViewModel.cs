using System;

namespace ARSFD.Web.Models.ManageViewModels
{
	public class WorkingHourListItemViewModel
	{
		public DayOfWeek DayOfWeek { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }
	}
}
