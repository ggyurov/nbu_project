using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Models.ManageViewModels;
using ARSFD.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ARSFD.Web.Controllers
{
	[Authorize]
	[Route("[controller]/[action]")]
	public class ManageController: Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IUserService _userService;
		private readonly IEmailSender _emailSender;
		private readonly ILogger _logger;
		private readonly UrlEncoder _urlEncoder;

		public ManageController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IUserService userService,
			IEmailSender emailSender,
			ILogger<ManageController> logger,
			UrlEncoder urlEncoder)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
			_emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_urlEncoder = urlEncoder ?? throw new ArgumentNullException(nameof(urlEncoder));
		}

		[TempData]
		public string StatusMessage { get; set; }

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var model = new IndexViewModel
			{
				Username = user.UserName,
				Email = user.Email,
				Names = user.Name,
				IsEmailConfirmed = user.EmailConfirmed,
				StatusMessage = StatusMessage
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Index(IndexViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var email = user.Email;
			if (model.Email != email)
			{
				var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
				if (!setEmailResult.Succeeded)
				{
					throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
				}
			}

			StatusMessage = "Your profile has been updated";
			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SendVerificationEmail(IndexViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
			var email = user.Email;
			await _emailSender.SendEmailConfirmationAsync(email, callbackUrl);

			StatusMessage = "Verification email sent. Please check your email.";
			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> ChangePassword()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);
			if (!hasPassword)
			{
				return RedirectToAction(nameof(SetPassword));
			}

			var model = new ChangePasswordViewModel { StatusMessage = StatusMessage };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				AddErrors(changePasswordResult);
				return View(model);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			_logger.LogInformation("User changed their password successfully.");
			StatusMessage = "Your password has been changed.";

			return RedirectToAction(nameof(ChangePassword));
		}

		[HttpGet]
		public async Task<IActionResult> WorkingHours(
			DayOfWeek? dayOfWeek = null,
			CancellationToken cancellationToken = default)
		{
			ApplicationUser user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			DayOfWeek[] filter = dayOfWeek != null
				? new DayOfWeek[] { dayOfWeek.Value }
				: null;

			IDictionary<DayOfWeek, WorkingHour[]> dictionary = await _userService.FindWorkingHours(user.Id, filter, cancellationToken);

			WorkingHourListItemViewModel[] workingHours = dictionary.Values
				.SelectMany(x => x.ToArray())
				.OrderBy(x => x.DayOfWeek)
				.ThenBy(x => x.StartTime)
				.Select(x => new WorkingHourListItemViewModel
				{
					Id = x.Id,
					DayOfWeek = x.DayOfWeek,
					StartTime = x.StartTime,
					EndTime = x.EndTime,
				})
				.ToArray();

			var model = new WorkingHoursViewModel
			{
				StatusMessage = StatusMessage,
				WorkingHours = workingHours,
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> AddWorkingHour(
			[FromForm(Name = "dayOfWeek")] DayOfWeek dayOfWeek,
			[FromForm(Name = "startTime")] DateTime startTime,
			[FromForm(Name = "endTime")] DateTime endTime,
			CancellationToken cancellationToken = default)
		{
			if (endTime.TimeOfDay < startTime.TimeOfDay
				|| startTime.TimeOfDay == endTime.TimeOfDay)
			{
				// TODO: add error
				return RedirectToAction(nameof(WorkingHours));
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var defaultDate = new DateTime(2001, 1, 1);

			var workingHour = new WorkingHour
			{
				UserId = user.Id,
				DayOfWeek = dayOfWeek,
				StartTime = defaultDate.Add(startTime.TimeOfDay),
				EndTime = defaultDate.Add(endTime.TimeOfDay),
			};

			await _userService.AddWorkingHour(workingHour, cancellationToken);

			return RedirectToAction(nameof(WorkingHours));
		}

		[HttpPost]
		public async Task<IActionResult> RemoveWorkingHour(
			[FromForm(Name = "id")] int id,
			CancellationToken cancellationToken = default)
		{
			await _userService.RemoveWorkingHour(id, cancellationToken);

			return RedirectToAction(nameof(WorkingHours));
		}

		[HttpPost]
		public async Task<IActionResult> AddToBlackList(
			[FromForm(Name = "id")] int id,
			CancellationToken cancellationToken = default)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await _userService.AddToBlackList(id, user.Id, cancellationToken);

			string referrer = Request.Headers[HeaderNames.Referer];
			return Redirect(referrer);
		}

		[HttpGet]
		public async Task<IActionResult> SetPassword()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);

			if (hasPassword)
			{
				return RedirectToAction(nameof(ChangePassword));
			}

			var model = new SetPasswordViewModel { StatusMessage = StatusMessage };
			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
			if (!addPasswordResult.Succeeded)
			{
				AddErrors(addPasswordResult);
				return View(model);
			}

			await _signInManager.SignInAsync(user, isPersistent: false);
			StatusMessage = "Your password has been set.";

			return RedirectToAction(nameof(SetPassword));
		}

		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		#endregion
	}
}
