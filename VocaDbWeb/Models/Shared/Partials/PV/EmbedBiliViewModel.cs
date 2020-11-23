using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Models.Shared.Partials.PV
{
	public class EmbedBiliViewModel
	{
		public EmbedBiliViewModel(PVContract pv, int width, int height)
		{
			PV = pv;
			Width = width;
			Height = height;
		}

		public PVContract PV { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }
	}
}