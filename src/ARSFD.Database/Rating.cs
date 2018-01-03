namespace ARSFD.Database
{
	public class Rating
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public int ByUserId { get; set; }

		public bool Value { get; set; }
	}
}
