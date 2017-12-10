using System;
using System.Threading;
using System.Threading.Tasks;

namespace ARSFD.Services.Impl
{
	public class AppointmentService: IAppointmentService
	{
		#region IAppointmentService members

		public async Task<FindResult<Appointment>> Find(
			FindAppointmentsFilter filter = null,
			FindOptions options = null,
			CancellationToken cancellationToken = default)
		{
			// TODO:
			await Task.CompletedTask;

			throw new NotImplementedException();
		}

		#endregion
	}
}
