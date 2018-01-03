using System;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using Microsoft.AspNetCore.Identity;

namespace ARSFD.Web.Services
{
	public class ApplicationUserStore: IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IRoleStore<ApplicationRole>
	{
		private readonly IUserService _userService;

		public ApplicationUserStore(IUserService userService)
		{
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

		#region IUserStore<ApplicationUser> members

		public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			await _userService.Create(user, cancellationToken);

			return IdentityResult.Success;
		}

		public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			try
			{
				int id = int.Parse(userId);

				ApplicationUser user = await _userService.Get(id, cancellationToken);

				return user;
			}
			catch (ServiceException)
			{
				// TODO:
				return null;
			}
		}

		public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			try
			{
				ApplicationUser user = await _userService.GetNormalized(normalizedUserName, cancellationToken);

				return user;
			}
			catch (ServiceException)
			{
				// TODO:
				return null;
			}
		}

		public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
			=> Task.FromResult(user.NormalizedUserName);

		public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
			=> Task.FromResult(user.Id.ToString());

		public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
			=> Task.FromResult(user.UserName);

		public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
		{
			user.NormalizedUserName = normalizedName;

			return Task.CompletedTask;
		}

		public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
		{
			user.UserName = userName;

			return Task.CompletedTask;
		}

		public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		#endregion

		#region IUserPasswordStore<ApplicationUser> members

		public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
		{
			user.PasswordHash = passwordHash;
			return Task.CompletedTask;
		}

		public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
			=> Task.FromResult(user.PasswordHash);

		public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
			=> Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));

		#endregion

		#region IRoleStore<ApplicationRole> members

		public Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
			=> Task.FromResult(role.Value.ToString());

		public Task<string> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
			=> Task.FromResult(Enum.GetName(typeof(ARSFD.Services.ApplicationRole), role.Value));

		public Task SetRoleNameAsync(ApplicationRole role, string roleName, CancellationToken cancellationToken)
			=> Task.CompletedTask;

		public Task<string> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
			=> Task.FromResult(Enum.GetName(typeof(ARSFD.Services.ApplicationRole), role.Value));

		public Task SetNormalizedRoleNameAsync(ApplicationRole role, string normalizedName, CancellationToken cancellationToken)
			=> Task.CompletedTask;

		Task<ApplicationRole> IRoleStore<ApplicationRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			ARSFD.Services.ApplicationRole value = Enum
				.Parse<ARSFD.Services.ApplicationRole>(roleId);

			var role = new ApplicationRole
			{
				Value = value,
			};

			return Task.FromResult(role);
		}

		Task<ApplicationRole> IRoleStore<ApplicationRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			ARSFD.Services.ApplicationRole value = Enum
				.Parse<ARSFD.Services.ApplicationRole>(normalizedRoleName);

			var role = new ApplicationRole
			{
				Value = value,
			};

			return Task.FromResult(role);
		}

		#endregion
	}
}
