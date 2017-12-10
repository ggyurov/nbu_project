using System;
using System.ComponentModel.DataAnnotations;

namespace ARSFD.Database
{
	public class Event
	{
		public int Id { get; set; }

		[MaxLength(128)]
		public string Title { get; set; }

		[MaxLength(256)]
		public string Text { get; set; }

		[MaxLength(450)]
		public string UserId { get; set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }
	}
}
