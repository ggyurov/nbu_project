using System;
using System.Threading;
using System.Threading.Tasks;
using DATABASE = ARSFD.Database;

namespace ARSFD.Services.Impl
{
	public class CommentService: ICommentService
	{
		private DATABASE.ApplicationDbContext _context;

		public CommentService(
			DATABASE.ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		#region ICommentService members

		#endregion
	}
}
