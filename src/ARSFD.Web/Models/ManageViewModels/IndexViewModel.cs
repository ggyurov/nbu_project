using System.ComponentModel.DataAnnotations;

namespace ARSFD.Web.Models.ManageViewModels
{
	public class IndexViewModel
	{
		[Display(Name = "Потребителско име")]
		public string Username { get; set; }

		public bool IsEmailConfirmed { get; set; }

		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[EmailAddress]
		[Display(Name = "Имейл")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[Display(Name = "Имена")]
		public string Names { get; set; }

		public string StatusMessage { get; set; }
	}
}
