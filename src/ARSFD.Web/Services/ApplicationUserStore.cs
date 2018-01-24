using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using Microsoft.AspNetCore.Identity;

namespace ARSFD.Web.Services
{
	public class ApplicationUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserClaimStore<ApplicationUser>
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

		public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			await _userService.UpdatePassword(user.Id, user.PasswordHash, cancellationToken);

			return IdentityResult.Success;
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

		#region IUserClaimStore<ApplicationUser> members

		public Task<IList<Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			string roleName = Enum.GetName(typeof(ARSFD.Services.RoleType), user.Role);

			var claims = new List<Claim>();
			var claim = new Claim(ApplicationRole.ClaimType, roleName);
			claims.Add(claim);

			return Task.FromResult<IList<Claim>>(claims);
		}

		public Task AddClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task ReplaceClaimAsync(ApplicationUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IList<ApplicationUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
