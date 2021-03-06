﻿using System.ComponentModel.DataAnnotations;
using ARSFD.Web.Models.CommentViewModels;

namespace ARSFD.Web.Models.DentistViewModels
{
	public class DentistViewModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string City { get; set; }

		public string Type { get; set; }

		public double Rating { get; set; }

		public CommentViewModel[] Comments { get; set; }

		public bool IsBlackListed { get; set; }

		public bool IsRated { get; set; }

		public int RateValue { get; set; }

		[Required(ErrorMessage = "Полето `{0}` е задължително.")]
		[Display(Name = "Мнение")]
		public string CommentText { get; set; }
	}
}
