using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DATABASE = ARSFD.Database;

namespace ARSFD.Services.Impl
{
	public class UserService : IUserService
	{
		private DATABASE.ApplicationDbContext _context;

		public UserService(
			DATABASE.ApplicationDbContext context)
		{
			_context = context ?? throw new ArgumentNullException(nameof(context));
		}

		#region IUserService members

		public async Task Create(
			ApplicationUser user,
			CancellationToken cancellationToken = default)
		{
			try
			{
				#region Validation

				if (user == null)
				{
					throw new ArgumentNullException(nameof(user));
				}

				if (user.UserName == default)
				{
					throw new ArgumentException("Invalid user name.");
				}

				if (user.Email == null)
				{
					throw new ArgumentException("Invalid user email..");
				}

				if (user.PasswordHash == null)
				{
					throw new ArgumentException("Invalid user password hash.");
				}

				// TODO: 

				#endregion

				DATABASE.ApplicationRole role = ConvertRole(user.Role);

				var app = new DATABASE.ApplicationUser
				{
					City = user.City,
					Email = user.Email,
					EmailConfirmed = user.EmailConfirmed,
					PasswordHash = user.PasswordHash,
					UserName = user.UserName,
					NormalizedUserName = user.NormalizedUserName,
					Role = role,
					Name = user.Name,
					Type = user.Type,
				};

				await _context.Users.AddAsync(app, cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException("Failed to create user.", ex);
			}
		}

		public async Task<ApplicationUser> Get(
			int id,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.ApplicationUser user = await _context
					.Users
					.FirstAsync(x => x.Id == id, cancellationToken);

				int[] votes = _context.Ratings
					.Where(x => x.UserId == id)
					.Select(x => x.Value)
					.ToArray();

				double rating = votes != null && votes.Length > 0
					? votes.Average()
					: 0;

				ApplicationUser app = ConvertUser(user, rating);

				return app;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get user with identifier `{id}`.", ex);
			}
		}

		public async Task<ApplicationUser> Get(
			string userName,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.ApplicationUser user = await _context
					.Users
					.FirstAsync(x => x.UserName == userName, cancellationToken);

				int[] votes = _context.Ratings
					.Where(x => x.UserId == user.Id)
					.Select(x => x.Value)
					.ToArray();

				double rating = votes != null && votes.Length > 0
					? votes.Average()
					: 0;

				ApplicationUser app = ConvertUser(user, rating);

				return app;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get user with userName `{userName}`.", ex);
			}
		}

		public async Task<ApplicationUser> GetNormalized(
			string normalizedUserName,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.ApplicationUser user = await _context
					.Users
					.FirstAsync(x => x.NormalizedUserName == normalizedUserName, cancellationToken);

				int[] votes = _context.Ratings
					.Where(x => x.UserId == user.Id)
					.Select(x => x.Value)
					.ToArray();

				double rating = votes != null && votes.Length > 0
					? votes.Average()
					: 0;

				ApplicationUser app = ConvertUser(user, rating);

				return app;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get user with normalizedUserName `{normalizedUserName}`.", ex);
			}
		}

		public async Task<ApplicationUser[]> FindDentists(
			string name,
			string city,
			string type,
			double? rating,
			CancellationToken cancellationToken = default)
		{
			try
			{
				IQueryable<DATABASE.ApplicationUser> users = _context.Users.Where(x => x.Role == DATABASE.ApplicationRole.Doctor);

				if (name != null)
				{
					users = users.Where(x => x.Name.Contains(name));
				}

				if (city != null)
				{
					users = users.Where(x => x.City.Contains(city));
				}

				if (type != null)
				{
					users = users.Where(x => x.Type.Contains(type));
				}

				if (rating != null)
				{
					int[] usersIds = await (
						from u in users
						join ur in _context.Ratings
							on u.Id equals ur.UserId
						group ur by ur.UserId into grp
						where grp.Average(x => x.Value) > rating
						select new
						{
							UserId = grp.Key,
							Rating = grp.Average(x => x.Value)
						})
						.Select(x => x.UserId)
						.ToArrayAsync(cancellationToken);

					users = users.Where(x => usersIds.Contains(x.Id));
				}

				Dictionary<int, double> ratings = await (
					from u in users
					join ur in _context.Ratings
						on u.Id equals ur.UserId
					group ur by ur.UserId into grp
					select new
					{
						UserId = grp.Key,
						Rating = grp.Average(x => x.Value)
					}
				).ToDictionaryAsync(k => k.UserId, v => v.Rating, cancellationToken);

				return await users
					.Select(x => ConvertUser(x, 0, ratings))
					.ToArrayAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get doctors", ex);
			}
		}

		public async Task<ApplicationUser[]> FindPatients(
			string name,
			double? rating,
			CancellationToken cancellationToken = default)
		{
			try
			{
				IQueryable<DATABASE.ApplicationUser> users = _context.Users.Where(x => x.Role == DATABASE.ApplicationRole.Patient);

				if (name != null)
				{
					users = users.Where(x => x.Name.Contains(name));
				}

				if (rating != null)
				{
					int[] usersIds = await (
						from u in users
						join ur in _context.Ratings
							on u.Id equals ur.UserId
						group ur by ur.UserId into grp
						where grp.Average(x => x.Value) > rating
						select new
						{
							UserId = grp.Key,
							Rating = grp.Average(x => x.Value)
						})
						.Select(x => x.UserId)
						.ToArrayAsync(cancellationToken);

					users = users.Where(x => usersIds.Contains(x.Id));
				}

				Dictionary<int, double> ratings = await (
					from u in users
					join ur in _context.Ratings
						on u.Id equals ur.UserId
					group ur by ur.UserId into grp
					select new
					{
						UserId = grp.Key,
						Rating = grp.Average(x => x.Value)
					}
				).ToDictionaryAsync(k => k.UserId, v => v.Rating, cancellationToken);

				return await users
					.Select(x => ConvertUser(x, 0, ratings))
					.ToArrayAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get patients", ex);
			}
		}

		public async Task<IDictionary<DayOfWeek, WorkingHour[]>> FindWorkingHours(
			int userId,
			DayOfWeek[] days = null,
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (days == null)
				{
					days = Enum
						.GetValues(typeof(DayOfWeek))
						.Cast<DayOfWeek>()
						.ToArray();
				}
				else
				{
					days = days
						.Distinct()
						.ToArray();
				}

				IQueryable<DATABASE.WorkingHour> entities = _context
					.WorkingHours
					.Where(x => x.UserId == userId && days.Contains(x.DayOfWeek));

				WorkingHour[] hours = await entities
					.Select(x => ConvertWorkingHour(x))
					.ToArrayAsync(cancellationToken);

				IDictionary<DayOfWeek, WorkingHour[]> dictionary = hours
					.GroupBy(x => x.DayOfWeek)
					.ToDictionary(k => k.Key, v => v.ToArray());

				return dictionary;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to find working hours for user `{userId}`.", ex);
			}
		}

		public async Task AddWorkingHour(
			WorkingHour workingHour,
			CancellationToken cancellationToken = default)
		{
			try
			{
				// TODO: validation
				var value = new DATABASE.WorkingHour
				{
					DayOfWeek = workingHour.DayOfWeek,
					UserId = workingHour.UserId,
					StartTime = workingHour.StartTime,
					EndTime = workingHour.EndTime,
				};

				_context.WorkingHours.Add(value);
				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to add working hour for user `{workingHour.UserId}`.", ex);
			}
		}

		public async Task RemoveWorkingHour(
			int id,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.WorkingHour entity = await _context.WorkingHours
					.FirstAsync(x => x.Id == id, cancellationToken);

				_context.WorkingHours.Remove(entity);

				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to remove working hour `{id}`.", ex);
			}
		}

		public async Task AddToBlackList(
			int userId,
			int byUserId,
			CancellationToken cancellationToken = default)
		{
			try
			{
				bool blacklisted = await _context.BlackLists
					.AnyAsync(x => x.UserId == userId && x.ByUserId == byUserId, cancellationToken);

				if (blacklisted)
				{
					throw new Exception("Already blacklisted.");
				}

				var item = new DATABASE.BlackList
				{
					UserId = userId,
					ByUserId = byUserId,
				};

				await _context.BlackLists.AddAsync(item, cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to add to blacklist.", ex);
			}
		}

		public async Task RemoveFromBlackList(
			int userId,
			int byUserId,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.BlackList item = await _context.BlackLists
					.FirstAsync(x => x.UserId == userId && x.ByUserId == byUserId, cancellationToken);

				_context.BlackLists.Remove(item);

				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to remove to blacklist.", ex);
			}
		}

		public async Task<BlackList[]> GetUserBlackLists(
			int id,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.BlackList[] items = await _context.BlackLists
					.Where(x => x.UserId == id)
					.ToArrayAsync(cancellationToken);

				return items.Select(x => new BlackList
				{
					Id = x.Id,
					ByUserId = x.ByUserId,
					UserId = x.UserId,
				}).ToArray();
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get to blacklists.", ex);
			}
		}

		public async Task<BlackList[]> GetByUserBlackLists(
			int id,
			CancellationToken cancellationToken = default)
		{
			try
			{
				DATABASE.BlackList[] items = await _context.BlackLists
					.Where(x => x.ByUserId == id)
					.ToArrayAsync(cancellationToken);

				return items.Select(x => new BlackList
				{
					Id = x.Id,
					ByUserId = x.ByUserId,
					UserId = x.UserId,
				}).ToArray();
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get to blacklists.", ex);
			}
		}

		public async Task UpdateNames(
			int userId,
			string names,
			CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrEmpty(names))
				{
					throw new ArgumentNullException(nameof(names));
				}

				DATABASE.ApplicationUser user = await _context.Users
					.FirstAsync(x => x.Id == userId, cancellationToken);

				// Update name
				user.Name = names;

				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to update user names.", ex);
			}
		}

		public async Task UpdatePassword(int userId, string hash, CancellationToken cancellationToken = default)
		{
			try
			{
				if (string.IsNullOrEmpty(hash))
				{
					throw new ArgumentNullException(nameof(hash));
				}

				DATABASE.ApplicationUser user = await _context.Users
					.FirstAsync(x => x.Id == userId, cancellationToken);

				// Update name
				user.PasswordHash = hash;

				await _context.SaveChangesAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to update user names.", ex);
			}
		}

		#endregion

		private static WorkingHour ConvertWorkingHour(DATABASE.WorkingHour value)
		{
			var workingHour = new WorkingHour
			{
				DayOfWeek = value.DayOfWeek,
				EndTime = value.EndTime,
				StartTime = value.StartTime,
				UserId = value.UserId,
				Id = value.Id,
			};

			return workingHour;
		}

		private static ApplicationUser ConvertUser(DATABASE.ApplicationUser value,
			double rating = 0, IDictionary<int, double> ratingMap = null)
		{
			RoleType role = ConvertRole(value.Role);

			var user = new ApplicationUser
			{
				Id = value.Id,
				City = value.City,
				Email = value.Email,
				EmailConfirmed = value.EmailConfirmed,
				PasswordHash = value.PasswordHash,
				UserName = value.UserName,
				NormalizedUserName = value.NormalizedUserName,
				Role = role,
				Name = value.Name,
				Type = value.Type,
				Rating = rating,
			};

			if (ratingMap != null
				&& ratingMap.TryGetValue(value.Id, out rating))
			{
				user.Rating = rating;
			}

			return user;
		}

		private static DATABASE.ApplicationRole ConvertRole(RoleType value)
		{
			switch (value)
			{
				case RoleType.None:
					return DATABASE.ApplicationRole.None;
				case RoleType.Patient:
					return DATABASE.ApplicationRole.Patient;
				case RoleType.Doctor:
					return DATABASE.ApplicationRole.Doctor;
				default:
					throw new ArgumentException($"Invalid application role `{value}`");
			}
		}

		private static RoleType ConvertRole(DATABASE.ApplicationRole value)
		{
			switch (value)
			{
				case DATABASE.ApplicationRole.None:
					return RoleType.None;
				case DATABASE.ApplicationRole.Patient:
					return RoleType.Patient;
				case DATABASE.ApplicationRole.Doctor:
					return RoleType.Doctor;
				default:
					throw new ArgumentException($"Invalid application role `{value}`");
			}
		}
	}
}
