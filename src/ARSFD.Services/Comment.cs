namespace ARSFD.Services
{
	public class Comment
	{
		public int Id { get; set; }
		
		public string Text { get; set; }

		public int? UserId { get; set; }

		public int ByUserId { get; set; }

		public int? EventId { get; set; }
	}
}
