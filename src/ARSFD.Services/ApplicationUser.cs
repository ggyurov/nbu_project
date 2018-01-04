using System.ComponentModel.DataAnnotations;

namespace ARSFD.Services
{
	public class ApplicationUser
	{
		public int Id { get; set; }

		public string PasswordHash { get; set; }

		[MaxLength(256)]
		public string Email { get; set; }

		[MaxLength(256)]
		public string UserName { get; set; }

		[MaxLength(256)]
		public string NormalizedUserName { get; set; }

		[MaxLength(256)]
		public string City { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		[MaxLength(256)]
		public string Type { get; set; }

		public bool EmailConfirmed { get; set; }

		public RoleType Role { get; set; }

		public double Rating { get; set; }
	}
}
