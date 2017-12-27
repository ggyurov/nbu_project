using System;
using System.Threading;
using System.Threading.Tasks;
using DATABASE = ARSFD.Database;

namespace ARSFD.Services.Impl
{
	public class EventService: IEventService
	{
		private DATABASE.ApplicationDbContext _context;

		public EventService(
			DATABASE.ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		#region IEventService members

		#endregion
	}
}
