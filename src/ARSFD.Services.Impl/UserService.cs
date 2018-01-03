using System;
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

				ApplicationRole role = ConvertRole(user.Role);

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

				ApplicationRole role = ConvertRole(user.Role);

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

				ApplicationRole role = ConvertRole(user.Role);

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
				};

				return app;
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get user with normalizedUserName `{normalizedUserName}`.", ex);
			}
		}

		public async Task<ApplicationUser[]> Find(string name, string city, string type, double? rating, CancellationToken cancellationToken = default)
		{
			try
			{
				IQueryable<DATABASE.ApplicationUser> users = _context.Users;

				if(name != null)
				{
					//users.Where(x => x.Name == name);
				}
				if(city != null)
				{
					users.Where(x => x.City.Contains(city));
				}
				if (type != null)
				{
					//users.Where(x => x.Type.Contains(type));
				}
				if (rating != null)
				{
					users = (
						from u in users
						join ur in _context.Ratings
							on u.Id equals ur.UserId
						//where ur.Value >= rating
						select u);
				}

				return await users.ToArrayAsync();
			}
			catch (Exception ex)
			{
				throw new ServiceException($"Failed to get users", ex);
			}
		}

		#endregion

		private static DATABASE.ApplicationRole ConvertRole(ApplicationRole value)
		{
			switch (value)
			{
				case ApplicationRole.None:
					return DATABASE.ApplicationRole.None;
				case ApplicationRole.Patient:
					return DATABASE.ApplicationRole.Patient;
				case ApplicationRole.Doctor:
					return DATABASE.ApplicationRole.Doctor;
				default:
					throw new ArgumentException($"Invalid application role `{value}`");
			}
		}

		private static ApplicationRole ConvertRole(DATABASE.ApplicationRole value)
		{
			switch (value)
			{
				case DATABASE.ApplicationRole.None:
					return ApplicationRole.None;
				case DATABASE.ApplicationRole.Patient:
					return ApplicationRole.Patient;
				case DATABASE.ApplicationRole.Doctor:
					return ApplicationRole.Doctor;
				default:
					throw new ArgumentException($"Invalid application role `{value}`");
			}
		}
	}
}
