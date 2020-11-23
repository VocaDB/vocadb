using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Models.Shared.Partials.PV
{

	public class EmbedPiaproViewModel
	{

		public EmbedPiaproViewModel(PVContract pv, string widthStr, string heightStr)
		{
			PV = pv;
			WidthStr = widthStr;
			HeightStr = heightStr;
		}

		public PVContract PV { get; set; }

		public string WidthStr { get; set; }

		public string HeightStr { get; set; }

	}

}