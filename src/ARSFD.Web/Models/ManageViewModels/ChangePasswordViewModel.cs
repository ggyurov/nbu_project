using System.ComponentModel.DataAnnotations;

namespace ARSFD.Web.Models.ManageViewModels
{
	public class ChangePasswordViewModel
	{
		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[DataType(DataType.Password)]
		[Display(Name = "Текуща парола")]
		public string OldPassword { get; set; }

		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[StringLength(100, ErrorMessage = "`{0}` трябва да бъде от `{1}` до `{2}` символа.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Нова парола")]
		public string NewPassword { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Повтори нова парола")]
		[Compare("NewPassword", ErrorMessage = "Паролата не съвпада.")]
		public string ConfirmPassword { get; set; }

		public string StatusMessage { get; set; }
	}
}
