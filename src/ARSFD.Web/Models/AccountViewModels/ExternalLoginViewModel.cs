using System.ComponentModel.DataAnnotations;

namespace ARSFD.Web.Models.AccountViewModels
{
	public class ExternalLoginViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
