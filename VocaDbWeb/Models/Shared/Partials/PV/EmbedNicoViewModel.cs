namespace VocaDb.Web.Models.Shared.Partials.PV {

	public class EmbedNicoViewModel {

		public EmbedNicoViewModel(string pvId, string widthStr, string heightStr, string id = null, bool enableApi = false) {
			PVId = pvId;
			WidthStr = widthStr;
			HeightStr = heightStr;
			Id = id;
			EnableApi = enableApi;
		}

		public string PVId { get; set; }

		public string WidthStr { get; set; }

		public string HeightStr { get; set; }

		public string Id { get; set; }

		public bool EnableApi { get; set; }

	}

}