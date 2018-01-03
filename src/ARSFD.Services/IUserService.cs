using System.Threading;
using System.Threading.Tasks;

namespace ARSFD.Services
{
	public interface IUserService
	{
		Task Create(
			ApplicationUser user,
			CancellationToken cancellationToken = default);

		Task<ApplicationUser> Get(
			int id,
			CancellationToken cancellationToken = default);

		Task<ApplicationUser> Get(
			string userName,
			CancellationToken cancellationToken = default);

		Task<ApplicationUser> GetNormalized(
			string normalizedNserName,
			CancellationToken cancellationToken = default);

		Task<ApplicationUser> Find(
			string name,
			string city,
			string type,
			double? rating,
			CancellationToken cancellationToken = default);
	}
}
