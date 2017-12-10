using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ARSFD.Database
{
	public class ApplicationUser: IdentityUser<string>
	{
		#region Navigation Properties

		public ICollection<IdentityUserClaim<string>> Claims { get; set; } = new HashSet<IdentityUserClaim<string>>();

		public ICollection<IdentityUserLogin<string>> Logins { get; set; } = new HashSet<IdentityUserLogin<string>>();

		public ICollection<IdentityUserRole<string>> Roles { get; set; } = new HashSet<IdentityUserRole<string>>();

		#endregion
	}
}
