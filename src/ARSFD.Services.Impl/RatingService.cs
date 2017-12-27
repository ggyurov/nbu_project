using System;
using System.Threading;
using System.Threading.Tasks;
using DATABASE = ARSFD.Database;

namespace ARSFD.Services.Impl
{
	public class RatingService: IRatingService
	{
		private DATABASE.ApplicationDbContext _context;

		public RatingService(
			DATABASE.ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		#region IRatingService members

		#endregion
	}
}
