namespace ARSFD.Services
{
	public class FindOptions
	{
		public FindOrderItem[] OrderItems { get; set; }

		public int? SkipCount { get; set; }

		public int? TakeCount { get; set; }
	}
}
