namespace VocaDb.Web.Models.Shared.Partials.Shared {

	public class ShowMoreViewModel {

		public ShowMoreViewModel(string js = null, string href = null) {
			JS = js;
			Href = href;
		}

		public string JS { get; set; }

		public string Href { get; set; }

	}

}