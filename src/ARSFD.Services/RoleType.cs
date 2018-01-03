using System;

namespace ARSFD.Services
{
	/// <summary>
	/// Enumeration of application user roles.
	/// </summary>
	[Flags]
	public enum RoleType: int
	{
		/// <summary>
		/// Flag indicating user has no role.
		/// </summary>
		None = 0,

		/// <summary>
		/// Flag indicating user is a doctor.
		/// </summary>
		Doctor = 1,

		/// <summary>
		/// Flag indicating user is a patient.
		/// </summary>
		Patient = 2,
	}
}
