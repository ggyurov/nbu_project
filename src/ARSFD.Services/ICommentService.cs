using System.Threading;
using System.Threading.Tasks;

namespace ARSFD.Services
{
	public interface ICommentService
	{
		Task<Comment[]> GetCommentsForUser(int userId, CancellationToken cancellationToken);
	}
}
