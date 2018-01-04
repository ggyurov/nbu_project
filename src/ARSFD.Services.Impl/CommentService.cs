using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

		public async Task<Comment[]> GetCommentsForUser(int userId, CancellationToken cancellationToken)
		{
			try
			{
				DATABASE.Comment[] entites = await _context.Comments.Where(x => x.UserId == userId).ToArrayAsync();

				Comment[] comments = entites.Select(x => ConvertComment(x)).ToArray();

				return comments;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get comments for user with ID `{userId}`.", ex);
			}
		}

		private Comment ConvertComment(DATABASE.Comment value)
		{
			var comment = new Comment
			{
				ByUserId = value.ByUserId,
				EventId = value.EventId,
				Id = value.Id,
				Text = value.Text,
				UserId = value.UserId
			};

			return comment;
		}

		#endregion
	}
}
