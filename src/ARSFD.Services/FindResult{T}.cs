namespace ARSFD.Services
{
	/// <summary>
	/// Represents a result of finding entities of a given type.
	/// </summary>
	/// <typeparam name="T">type of entities</typeparam>
	public class FindResult<T>
	{
		/// <summary>
		/// Gets or sets the total number of items in the result.
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Gets or sets the items in the result.
		/// </summary>
		public T[] Items { get; set; }

		public FindResult()
		{

		}

		public FindResult(T[] items)
		{
			Items = items;
			TotalCount = items.Length;
		}
	}
}
