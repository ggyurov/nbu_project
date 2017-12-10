using System.Threading;
using System.Threading.Tasks;

namespace ARSFD.Services
{
	public interface IAppointmentService
	{
		Task<FindResult<Appointment>> Find(
			FindAppointmentsFilter filter = null,
			FindOptions options = null,
			CancellationToken cancellationToken = default);
	}
}
