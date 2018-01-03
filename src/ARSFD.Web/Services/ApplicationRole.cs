using SERVICES = ARSFD.Services;

namespace ARSFD.Web.Services
{
	public class ApplicationRole
	{
		public const string ClaimType = "RoleClaim";

		public SERVICES.RoleType Value { get; set; }
	}
}
