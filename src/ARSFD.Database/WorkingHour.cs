using System;

namespace ARSFD.Database
{
	public class WorkingHour
	{
		public int Id { get; set; }

		public int UserId { get; set; }

		public DayOfWeek DayOfWeek { get; set; }

		public DateTime StartTime { get; set; }

		public DateTime EndTime { get; set; }
	}
}
