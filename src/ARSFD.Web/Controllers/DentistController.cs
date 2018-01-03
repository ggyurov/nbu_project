using System.Threading;
using System.Threading.Tasks;
using ARSFD.Web.Models.DentistViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ARSFD.Web.Controllers
{
	[Route("dentist")]
	public class DentistController : Controller
	{
		[HttpGet]
		[Route("")]
		public IActionResult Index(
			[FromQuery(Name ="Name")] string name, 
			[FromQuery(Name = "City")] string city, 
			[FromQuery(Name = "Type")] string type,
			[FromQuery(Name = "Rating")] string rating)
		{
			var model = new DentistIndexViewModel
			{
				Dentists = new[] { new DentistListItemViewModel { Id = "1", Name = "kiro", City = "plovdiv", Rating = 5.5, Type = "Ortodont" } },
			};

			return View(model);
		}

		[Route("{id}")]
		public async Task<IActionResult> Single(
			[FromRoute(Name ="id")] string id,
			CancellationToken cancellationToken)
		{

			return View();
		}
	}
}