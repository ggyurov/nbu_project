using System;
using Microsoft.AspNetCore.Mvc;
using SmartFuel.Web.ViewModels;

namespace ARSFD.Web.Extensions
{
	public static class Extensions
	{
		public static ApplicationExceptionViewModel ToApplicationExceptionViewModel(this Exception ex)
		{
			var model = new ApplicationExceptionViewModel
			{
				Message = ex.Message,
				TypeName = ex.GetType().Name,
				Exception = ex,
				InnerException = ex.InnerException?.ToApplicationExceptionViewModel(),
				StackTrace = ex.StackTrace,
			};

			return model;
		}

		public static IActionResult HandleException(this Controller controller,
			Exception exception,
			string message = null)
		{
			if (controller == null)
			{
				throw new ArgumentNullException(nameof(controller));
			}

			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			IActionResult result;

			message = message ?? "An unknown error has occurred while processing a user request.";

			result = controller.StatusCode(500, exception.ToApplicationExceptionViewModel());

			return result;
		}
	}
}
