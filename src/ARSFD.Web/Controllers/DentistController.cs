using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Extensions;
using ARSFD.Web.Models.CommentViewModels;
using ARSFD.Web.Models.DentistViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ARSFD.Web.Controllers
{
	[Route("dentist")]
	[Authorize(nameof(RoleType.Patient))]
	public class DentistController: Controller
	{
		private readonly IUserService _userService;
		private readonly ICommentService _commentService;
		private readonly UserManager<ApplicationUser> _userManager;

		public DentistController(
			IUserService userService,
			ICommentService commentService,
			UserManager<ApplicationUser> userManager)
		{
			_userService = userService;
			_commentService = commentService;
			_userManager = userManager;
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
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
				}

				ApplicationUser dentist = await _userService.Get(id, cancellationToken);
				Comment[] comments = await _commentService.GetCommentsForUser(id, cancellationToken);

				BlackList[] blackLists = await _userService.GetUserBlackLists(id, cancellationToken);
				bool isBlackListed = blackLists.Any(x => x.ByUserId == user.Id);

				CommentViewModel[] commentsModel = comments.Select(x => new CommentViewModel
				{
					ByUserId = x.ByUserId,
					EventId = x.EventId,
					Id = x.Id,
					Text = x.Text,
					UserId = x.UserId,
					ByUserName = _userService.Get(x.ByUserId, cancellationToken).Result.Name,
				}).ToArray();

				var model = new DentistViewModel
				{
					City = dentist.City,
					Id = dentist.Id,
					Name = dentist.Name,
					Rating = dentist.Rating,
					Type = dentist.Type,
					Comments = commentsModel,
					IsBlackListed = isBlackListed,
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