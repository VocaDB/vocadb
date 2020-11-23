using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class PVInfoViewModel
	{
		public PVInfoViewModel(ArchivedPVContract pv)
		{
			PV = pv;
		}

		public ArchivedPVContract PV { get; set; }
	}
}