using System;
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

		#endregion

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
