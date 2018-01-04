using System.ComponentModel.DataAnnotations;

namespace ARSFD.Web.Models.AccountViewModels
{
	public class LoginViewModel
	{
		[EmailAddress]
		[Display(Name = "Имейл")]
		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		public string Email { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Парола")]
		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		public string Password { get; set; }

		[Display(Name = "Запомни ме?")]
		public bool RememberMe { get; set; }
	}
}
