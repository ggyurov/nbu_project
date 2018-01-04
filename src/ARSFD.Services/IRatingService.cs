using System.Threading;
using System.Threading.Tasks;

namespace ARSFD.Services
{
	public interface IRatingService
	{
		Task SetRating(int userId, int value, int byUserId, CancellationToken cancellationToken = default);

		Task<Rating[]> Get(int userId, CancellationToken cancellationToken = default);
	}
}
