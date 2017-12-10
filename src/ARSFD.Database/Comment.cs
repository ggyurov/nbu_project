using System.ComponentModel.DataAnnotations;

namespace ARSFD.Database
{
	public class Comment
	{
		public int Id { get; set; }

		[MaxLength(256)]
		public string Text { get; set; }

		[MaxLength(450)]
		public string UserId { get; set; }

		[MaxLength(450)]
		public string ByUserId { get; set; }

		public int? EventId { get; set; }
	}
}
