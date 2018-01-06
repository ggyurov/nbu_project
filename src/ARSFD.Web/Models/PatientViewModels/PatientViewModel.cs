using System.ComponentModel.DataAnnotations;
using ARSFD.Web.Models.CommentViewModels;

namespace ARSFD.Web.Models.PatientViewModels
{
	public class PatientViewModel
	{
		public int Id { get; set; }

		public string Name { get; set; }

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
