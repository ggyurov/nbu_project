using System;

namespace ARSFD.Services
{
	/// <summary>
	/// Represents an exception caused by a service.
	/// </summary>
	public class ServiceException: Exception
	{
		/// <summary>
		/// Creates a default <see cref="ServiceException"/> instance.
		/// </summary>
		public ServiceException()
		{
		}

		/// <summary>
		/// Creates a <see cref="ServiceException"/> instance with a specified message.
		/// </summary>
		public ServiceException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Creates a <see cref="ServiceException"/> instance with a specified message and inner exception.
		/// </summary>
		public ServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
