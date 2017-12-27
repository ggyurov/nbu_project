using System;
using System.Threading;
using System.Threading.Tasks;
using DATABASE = ARSFD.Database;

namespace ARSFD.Services.Impl
{
	public class UserService: IUserService
	{
		private DATABASE.ApplicationDbContext _context;

		public UserService(
			DATABASE.ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		#region IUserService members

		#endregion
	}
}
