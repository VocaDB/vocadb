using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class FormatPVLinkViewModel
	{
		public FormatPVLinkViewModel(PVContract pv, bool type = true)
		{
			PV = pv;
			Type = type;
		}

		public PVContract PV { get; set; }

		public bool Type { get; set; }
	}
}