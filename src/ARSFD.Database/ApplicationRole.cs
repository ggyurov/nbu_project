using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ARSFD.Database
{
	public class ApplicationRole: IdentityRole<string>
	{
		#region Navigation Properties

		public ICollection<IdentityRoleClaim<string>> Claims { get; set; } = new HashSet<IdentityRoleClaim<string>>();

		public ICollection<IdentityUserRole<string>> Users { get; set; } = new HashSet<IdentityUserRole<string>>();

		#endregion
	}
}
