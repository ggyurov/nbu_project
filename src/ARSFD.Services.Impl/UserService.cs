using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

				double rating = _context.Ratings.Where(x => x.UserId == id).Select(x => x.Value).DefaultIfEmpty(0).Average();

				RoleType role = ConvertRole(user.Role);

				var applicationUser = new ApplicationUser
				{
					Id = user.Id,
					City = user.City,
					Email = user.Email,
					EmailConfirmed = user.EmailConfirmed,
					PasswordHash = user.PasswordHash,
					UserName = user.UserName,
					NormalizedUserName = user.NormalizedUserName,
					Role = role,
					Name = user.Name,
					Type = user.Type,
					Rating = rating
				};

				return applicationUser;
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

				RoleType role = ConvertRole(user.Role);

				var app = new ApplicationUser
				{
					Id = user.Id,
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

				RoleType role = ConvertRole(user.Role);

				var app = new ApplicationUser
				{
					Id = user.Id,
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

				return app;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get user with normalizedUserName `{normalizedUserName}`.", ex);
			}
		}

		public async Task<ApplicationUser[]> FindDentists(string name, string city, string type, double? rating, CancellationToken cancellationToken = default)
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
					int[] usersIds = (
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
						.ToArray();

					users = users.Where(x => usersIds.Contains(x.Id));
				}

				return await users.Select(x => ConvertUser(x)).ToArrayAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get doctors", ex);
			}
		}

		public async Task<ApplicationUser[]> FindPatients(string name, double? rating, CancellationToken cancellationToken = default)
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
					int[] usersIds = (
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
						.ToArray();

					users = users.Where(x => usersIds.Contains(x.Id));
				}

				return await users.Select(x => ConvertUser(x)).ToArrayAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get patients", ex);
			}
		}

		public async Task<IDictionary<DayOfWeek, WorkingHour[]>> FindWorkingHours(int userId, DayOfWeek[] days = null, CancellationToken cancellationToken = default)
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

		private static ApplicationUser ConvertUser(DATABASE.ApplicationUser value)
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
			};

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
