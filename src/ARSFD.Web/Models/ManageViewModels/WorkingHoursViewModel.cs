using System.ComponentModel.DataAnnotations;

namespace ARSFD.Web.Models.ManageViewModels
{
	public class WorkingHoursViewModel
	{
		public CreateWorkingHourViewModel Add { get; set; } = new CreateWorkingHourViewModel();

		public WorkingHourListItemViewModel[] WorkingHours { get; set; }

		public string StatusMessage { get; set; }
	}
}
