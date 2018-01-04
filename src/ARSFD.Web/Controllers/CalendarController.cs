﻿using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using Microsoft.AspNetCore.Mvc;

namespace ARSFD.Web.Controllers
{
	[Route("calendar")]
	public class CalendarController: Controller
	{
		private readonly IUserService _userService;

		public CalendarController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> Index(
			CancellationToken cancellationToken = default)
		{
			return View();
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
