using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using ARSFD.Web.Extensions;
using ARSFD.Web.Models.AppointmentViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ARSFD.Web.Controllers
{
	[Route("appointment")]
	public class AppointmentController: Controller
	{
		public const string Name = "Appointment";

		private readonly IUserService _userService;
		private readonly IAppointmentService _appointmentService;
		private readonly UserManager<ApplicationUser> _userManager;

		public AppointmentController(
			IUserService userService,
			IAppointmentService appointmentService,
			 UserManager<ApplicationUser> userManager)
		{
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
			_appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
			_userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
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
			var model = new AppointmentIndexViewModel();

			return View(model);
		}

		[Route("{id}")]
		public async Task<IActionResult> Single(
			[FromRoute(Name = "id")] int id,
			CancellationToken cancellationToken = default)
		{

			return View();
		}

		[HttpPost]
		[Route("save")]
		[Authorize(nameof(RoleType.Patient))]
		public async Task<IActionResult> Save(
			[FromForm(Name = "userId")] int userId,
			[FromForm(Name = "date")] DateTime date,
			CancellationToken cancellationToken = default)
		{
			ApplicationUser user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var appointment = new Appointment
			{
				UserId = user.Id,
				Date = date,
				DoctorId = userId,
			};

			await _appointmentService.Create(appointment, cancellationToken);

			string referrer = Request.Headers[HeaderNames.Referer];
			return Redirect(referrer);
		}

		[HttpGet]
		[Route("free")]
		public async Task<IActionResult> FreeAppointments(
			[FromQuery(Name = "userId")] int userId,
			[FromQuery(Name = "date")] DateTime date,
			CancellationToken cancellationToken = default)
		{
			try
			{
				ApplicationUser user = await _userService.Get(userId, cancellationToken);

				if (user == null || user.Role != RoleType.Doctor)
				{
					return BadRequest("Invalid user.");
				}

				IDictionary<DayOfWeek, WorkingHour[]> workingHoursDictionary = await _userService
					.FindWorkingHours(userId, new[] { date.DayOfWeek }, cancellationToken);

				if (!workingHoursDictionary.TryGetValue(date.DayOfWeek, out WorkingHour[] workingHours)
					|| workingHours.Length <= 0)
				{
					return Ok(new FreeAppointmentViewModel[] { });
				}

				var freeAppointments = new List<FreeAppointmentViewModel>();

				DateTime dateValue = date.Date;

				foreach (WorkingHour hour in workingHours)
				{
					const double appointmentDuration = 30; // Minutes

					TimeSpan workingDuration = hour.EndTime - hour.StartTime;

					int appointmentCount = (int)Math.Floor(workingDuration.TotalMinutes / appointmentDuration);

					TimeSpan startTime = hour.StartTime.TimeOfDay;
					DateTime startDate = dateValue.Add(startTime);

					for (int i = 0; i < appointmentCount; i++)
					{
						DateTime endTime = startDate.AddMinutes(appointmentDuration);

						var freeItem = new FreeAppointmentViewModel
						{
							StartTime = startDate,
							EndTime = endTime,
						};

						freeAppointments.Add(freeItem);

						// increase start date
						startDate = endTime;
					}
				}

				var filter = new FindAppointmentsFilter
				{
					Date = date.Date,
					DoctorId = user.Id,
					Canceled = false,
				};

				FindResult<Appointment> appointments = await _appointmentService
					.Find(filter, cancellationToken: cancellationToken);

				if (appointments.TotalCount <= 0)
				{
					return Ok(freeAppointments.ToArray());
				}
				else
				{
					// filter saved appointments
					FreeAppointmentViewModel[] filteredAppointments = freeAppointments
						.Where(x => !appointments.Items.Any(y => y.Date == x.StartTime))
						.ToArray();

					return Ok(filteredAppointments);
				}
			}
			catch (Exception ex)
			{
				return this.HandleException(ex);
			}
		}
	}
}
