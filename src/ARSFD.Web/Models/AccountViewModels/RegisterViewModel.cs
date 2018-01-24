using System.ComponentModel.DataAnnotations;
using ARSFD.Services;

namespace ARSFD.Web.Models.AccountViewModels
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		public RoleType Role { get; set; }

		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[Display(Name = "Имена")]
		public string Name { get; set; }

		[Display(Name = "Населено място")]
		public string City { get; set; }

		[Display(Name = "Специалност")]
		public string Type { get; set; }

		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[EmailAddress]
		[Display(Name = "Имейл")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[StringLength(100, ErrorMessage = "`{0}` трябва да бъде от `{1}` до `{2}` символа.", MinimumLength = 5)]
		[DataType(DataType.Password)]
		[Display(Name = "Парола")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Повтори парола")]
		[Compare(nameof(Password), ErrorMessage = "Паролата не съвпада.")]
		public string ConfirmPassword { get; set; }
	}
}
