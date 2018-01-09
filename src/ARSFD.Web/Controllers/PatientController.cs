using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Extensions;
using ARSFD.Web.Models.CommentViewModels;
using ARSFD.Web.Models.PatientViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ARSFD.Web.Controllers
{
	[Route("patient")]
	[Authorize(nameof(RoleType.Doctor))]
	public class PatientController: Controller
	{

		private readonly IUserService _userService;
		private readonly ICommentService _commentService;
		private readonly IRatingService _ratingService;
		private readonly UserManager<ApplicationUser> _userManager;

		public PatientController(
			IUserService userService,
			ICommentService commentService,
			IRatingService ratingService,
			UserManager<ApplicationUser> userManager)
		{
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
			_commentService = commentService ?? throw new ArgumentNullException(nameof(commentService));
			_ratingService = ratingService ?? throw new ArgumentNullException(nameof(ratingService));
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
					Rating = x.Rating,
				}).ToArray();

			return View(model);
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

				Rating[] ratings = await _ratingService.Get(id, cancellationToken);
				Rating rating = ratings.FirstOrDefault(x => x.ByUserId == user.Id);
				bool isRated = rating != null;

				CommentViewModel[] commentsModel = comments.Select(x => new CommentViewModel
				{
					ByUserId = x.ByUserId,
					EventId = x.EventId,
					Id = x.Id,
					Text = x.Text,
					UserId = x.UserId,
					ByUserName = _userService.Get(x.ByUserId, cancellationToken).Result.Name,
				}).ToArray();

				var model = new PatientViewModel
				{
					Id = dentist.Id,
					Name = dentist.Name,
					Rating = dentist.Rating,
					Comments = commentsModel,
					IsBlackListed = isBlackListed,
					IsRated = isRated,
					RateValue = rating?.Value ?? 1,
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