using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Models.DentistViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ARSFD.Web.Controllers
{
	[Route("dentist")]
	public class DentistController : Controller
	{
		private readonly IUserService _userService;

		public DentistController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> Index(
			[FromQuery(Name = "Name")] string name,
			[FromQuery(Name = "City")] string city,
			[FromQuery(Name = "Type")] string type,
			[FromQuery(Name = "Rating")] double? rating,
			CancellationToken cancellationToken = default)
		{
			ApplicationUser[] dentists = await _userService.FindDentists(name, city, type, rating, cancellationToken);

			var model = new DentistIndexViewModel();

			model.Dentists = dentists.Select(x =>
				new DentistListItemViewModel
				{
					Id = x.Id,
					City = x.City,
					Name = x.Name,
					//Rating = x.Rating,
					Type = x.Type
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