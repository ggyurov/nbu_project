using System;

namespace ARSFD.Services
{
	public class FindAppointmentsFilter: PropertySet<FindAppointmentsFilter>
	{
		public DateTime Date
		{
			get => Get(x => x.Date);
			set => Set(x => x.Date, value);
		}

		public int UserId
		{
			get => Get(x => x.UserId);
			set => Set(x => x.UserId, value);
		}

		public int DoctorId
		{
			get => Get(x => x.DoctorId);
			set => Set(x => x.DoctorId, value);
		}

		public bool Canceled
		{
			get => Get(x => x.Canceled);
			set => Set(x => x.Canceled, value);
		}

		public int CanceledById
		{
			get => Get(x => x.CanceledById);
			set => Set(x => x.CanceledById, value);
		}
	}
}