#nullable disable

using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Web.Models.Shared.Partials.PV
{
	public class EmbedPVViewModel
	{
		public EmbedPVViewModel(PVContract pv, int width = 560, int height = 315, bool autoplay = false, bool enableApi = false, string id = null)
		{
			PV = pv;
			Width = width;
			Height = height;
			Autoplay = autoplay;
			EnableApi = enableApi;
			Id = id;
		}

		public PVContract PV { get; set; }

		public int Width { get; set; }

		public int Height { get; set; }

		public bool Autoplay { get; set; }

		public bool EnableApi { get; set; }

		public string Id { get; set; }
	}
}