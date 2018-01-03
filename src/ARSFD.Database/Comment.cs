using System.ComponentModel.DataAnnotations;

namespace ARSFD.Database
{
	public class Comment
	{
		public int Id { get; set; }

		[MaxLength(256)]
		public string Text { get; set; }

		public int? UserId { get; set; }

		public int ByUserId { get; set; }

		public int? EventId { get; set; }
	}
}
