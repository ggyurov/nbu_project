using System;

namespace ARSFD.Services
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
