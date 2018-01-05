using System;
using System.Collections.Generic;
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

		Task<ApplicationUser[]> FindDentists(
			string name,
			string city,
			string type,
			double? rating,
			CancellationToken cancellationToken = default);

		Task<ApplicationUser[]> FindPatients(
			string name,
			double? rating,
			CancellationToken cancellationToken = default);

		Task<IDictionary<DayOfWeek, WorkingHour[]>> FindWorkingHours(
			int userId,
			DayOfWeek[] days = null,
			CancellationToken cancellationToken = default);

		Task AddWorkingHour(
			WorkingHour workingHour,
			CancellationToken cancellationToken = default);

		Task RemoveWorkingHour(
			int id,
			CancellationToken cancellationToken = default);

		Task AddToBlackList(
			int userId,
			int byUserId,
			CancellationToken cancellationToken = default);

		Task RemoveFromBlackList(
			int userId,
			int byUserId,
			CancellationToken cancellationToken = default);

		Task<BlackList[]> GetUserBlackLists(
			int id,
			CancellationToken cancellationToken = default);

		Task<BlackList[]> GetByUserBlackLists(
			int id,
			CancellationToken cancellationToken = default);

		Task UpdateNames(
			int userId,
			string names,
			CancellationToken cancellationToken = default);
	}
}
