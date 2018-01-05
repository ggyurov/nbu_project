using System.Threading;
using System.Threading.Tasks;

namespace ARSFD.Services
{
	public interface ICommentService
	{
		Task Create(int userId, int byUserId, string text, CancellationToken cancellationToken);

		Task<Comment[]> GetCommentsForUser(int userId, CancellationToken cancellationToken);
	}
}
