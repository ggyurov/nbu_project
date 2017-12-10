using System.ComponentModel.DataAnnotations;

namespace ARSFD.Database
{
	public class Rating
	{
		public int Id { get; set; }

		[MaxLength(450)]
		public string UserId { get; set; }

		[MaxLength(450)]
		public string ByUserId { get; set; }

		public bool Value { get; set; }
	}
}
