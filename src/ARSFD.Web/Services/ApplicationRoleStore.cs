using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using ARSFD.Services;
using Microsoft.AspNetCore.Identity;

namespace ARSFD.Web.Services
{
	public class ApplicationRoleStore: IRoleStore<ApplicationRole>, IRoleClaimStore<ApplicationRole>
	{
		private readonly IUserService _userService;

		public ApplicationRoleStore(IUserService userService)
		{
			_userService = userService ?? throw new ArgumentNullException(nameof(userService));
		}

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

		public Task<ApplicationRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			ARSFD.Services.ApplicationRole value = Enum
				.Parse<ARSFD.Services.ApplicationRole>(roleId);

			var role = new ApplicationRole
			{
				Value = value,
			};

			return Task.FromResult(role);
		}

		public Task<ApplicationRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			ARSFD.Services.ApplicationRole value = Enum
				.Parse<ARSFD.Services.ApplicationRole>(normalizedRoleName);

			var role = new ApplicationRole
			{
				Value = value,
			};

			return Task.FromResult(role);
		}

		public void Dispose()
		{

		}

		#endregion

		#region IRoleClaimStore<ApplicationRole>

		public Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default)
		{
			string roleName = Enum.GetName(typeof(ARSFD.Services.ApplicationRole), role.Value);

			var claims = new List<Claim>();
			var claim = new Claim(ApplicationRole.ClaimType, roleName);
			claims.Add(claim);

			return Task.FromResult<IList<Claim>>(claims);
		}

		public Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		public Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
