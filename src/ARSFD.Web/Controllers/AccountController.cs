﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Models.AccountViewModels;
using ARSFD.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ARSFD.Web.Controllers
{
	[Authorize]
	[Route("[controller]/[action]")]
	public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly IUserService _userService;
		private readonly IEmailSender _emailSender;
		private readonly ILogger _logger;

		public AccountController(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			IUserService userService,
			IEmailSender emailSender,
			ILogger<AccountController> logger)
		{
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
			_signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
			_emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		[TempData]
		public string ErrorMessage { get; set; }

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string returnUrl = null)
		{
			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(
			LoginViewModel model,
			string returnUrl = null,
			CancellationToken cancellationToken = default)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (ModelState.IsValid)
			{
				ApplicationUser user = await _userManager.FindByNameAsync(model.Email);
				if (user != null)
				{
					BlackList[] blacklists = await _userService.GetUserBlackLists(user.Id, cancellationToken);

					// Check if user is blocked
					if (blacklists.Length >= 4)
					{
						ModelState.AddModelError(string.Empty, "User is blocked.");
						return View(model);
					}
				}

				var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");
					return RedirectToLocal(returnUrl);
				}

				ModelState.AddModelError(string.Empty, "Invalid login attempt.");
				return View(model);
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Register(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			var model = new RegisterViewModel
			{
				Role = RoleType.Patient
			};

			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (ModelState.IsValid)
			{
				if (model.Role == RoleType.Doctor)
				{
					if (string.IsNullOrEmpty(model.Type))
					{
						ModelState.AddModelError(nameof(model.Type), "Полето `Специалност` е задължително.");
					}

					if (string.IsNullOrEmpty(model.City))
					{
						ModelState.AddModelError(nameof(model.City), "Полето `Населено място` е задължително.");
					}

					if (!ModelState.IsValid)
					{
						return View(model);
					}
				}

				var user = new ApplicationUser
				{
					UserName = model.Email,
					Email = model.Email,
					Role = model.Role,
					City = model.City,
					Name = model.Name,
					Type = model.Type,
				};

				IdentityResult result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					// Refresh user to get the ID
					user = await _userManager.FindByNameAsync(user.UserName);

					await _emailSender.SendEmailAsync(user.Email, "Успешна регистрация", string.Empty);

					await _signInManager.SignInAsync(user, isPersistent: false);

					_logger.LogInformation("User created a new account with password.");

					return RedirectToLocal(returnUrl);
				}

				AddErrors(result);
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			_logger.LogInformation("User logged out.");
			return RedirectToAction(nameof(HomeController.Index), "Home");
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{userId}'.");
			}
			var result = await _userManager.ConfirmEmailAsync(user, code);
			return View(result.Succeeded ? "ConfirmEmail" : "Error");
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return RedirectToAction(nameof(ForgotPasswordConfirmation));
				}

				// For more information on how to enable account confirmation and password reset please
				// visit https://go.microsoft.com/fwlink/?LinkID=532713
				var code = await _userManager.GeneratePasswordResetTokenAsync(user);
				var callbackUrl = Url.ResetPasswordCallbackLink(user.Id, code, Request.Scheme);
				await _emailSender.SendEmailAsync(model.Email, "Reset Password",
					$"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
				return RedirectToAction(nameof(ForgotPasswordConfirmation));
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPassword(string code = null)
		{
			if (code == null)
			{
				throw new ApplicationException("A code must be supplied for password reset.");
			}
			var model = new ResetPasswordViewModel { Code = code };
			return View(model);
		}

		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}
			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}
			var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}
			AddErrors(result);
			return View();
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}


		[HttpGet]
		public IActionResult AccessDenied()
		{
			return View();
		}

		#region Helpers

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}

		#endregion
	}
}
