using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Extensions;
using ARSFD.Web.Models.CommentViewModels;
using ARSFD.Web.Models.DentistViewModels;
using ARSFD.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ARSFD.Web.Controllers
{
	[Route("dentist")]
	[Authorize(nameof(RoleType.Patient))]
	public class DentistController : Controller
	{
		private readonly IUserService _userService;
		private readonly ICommentService _commentService;

		public DentistController(IUserService userService, ICommentService commentService)
		{
			_userService = userService;
			_commentService = commentService;
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
			try
			{
				ApplicationUser[] dentists = await _userService.FindDentists(name, city, type, rating, cancellationToken);

				var model = new DentistIndexViewModel();

				model.Dentists = dentists.Select(x =>
					new DentistListItemViewModel
					{
						Id = x.Id,
						City = x.City,
						Name = x.Name,
						Rating = x.Rating,
						Type = x.Type
					}).ToArray();

				return View(model);
			}
			catch (Exception ex)
			{
				return this.HandleException(ex);
			}
		}

		[Route("{id}")]
		public async Task<IActionResult> Single(
			[FromRoute(Name = "id")] int id,
			CancellationToken cancellationToken = default)
		{
			try
			{
				ApplicationUser dentist = await _userService.Get(id, cancellationToken);
				Comment[] comments = await _commentService.GetCommentsForUser(id, cancellationToken);

				CommentViewModel[] commentsModel = comments.Select(x => new CommentViewModel
				{
					ByUserId = x.ByUserId,
					EventId = x.EventId,
					Id = x.Id,
					Text = x.Text,
					UserId = x.UserId,
					ByUserName =  _userService.Get(x.ByUserId, cancellationToken).Result.Name
				}).ToArray();

				var model = new DentistViewModel
				{
					City = dentist.City,
					Id = dentist.Id,
					Name = dentist.Name,
					Rating = dentist.Rating,
					Type = dentist.Type,
					Comments = commentsModel
				};

				return View(model);
			}
			catch (Exception ex)
			{
				return this.HandleException(ex);
			}
		}
	}
}