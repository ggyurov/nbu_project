using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Models.PatientViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ARSFD.Web.Controllers
{
	[Route("patient")]
	public class PatientController: Controller
	{
		private readonly IUserService _userService;

		public PatientController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> Index(
			[FromQuery(Name = "Name")] string name,
			[FromQuery(Name = "Rating")] double? rating,
			CancellationToken cancellationToken = default)
		{
			ApplicationUser[] patients = await _userService.FindPatients(name, rating, cancellationToken);

			var model = new PatientIndexViewModel();

			model.Patients = patients.Select(x =>
				new PatientListItemViewModel
				{
					Id = x.Id,
					Name = x.Name,
					//Rating = x.Rating,
				}).ToArray();

			return View(model);
		}

		[Route("{id}")]
		public async Task<IActionResult> Single(
			[FromRoute(Name = "id")] string id,
			CancellationToken cancellationToken = default)
		{

			return View();
		}
	}
}