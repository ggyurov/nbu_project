namespace ARSFD.Services
{
	/// <summary>
	/// Represents an ordering item.
	/// </summary>
	public class FindOrderItem
	{
		/// <summary>
		/// Gets or sets the property name to order by.
		/// </summary>
		public string PropertyName { get; set; }

		/// <summary>
		/// Gets or sets the flag indicating whether the order is descending or not.
		/// </summary>
		public bool IsDescending { get; set; }
	}
}
