﻿using System.ComponentModel.DataAnnotations;

namespace ARSFD.Web.Models.AccountViewModels
{
	public class ForgotPasswordViewModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}
