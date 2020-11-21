namespace VocaDb.Web.Models.Shared.Partials.Shared {

	public class SaveAndBackBtnViewModel {

		public SaveAndBackBtnViewModel(string backAction) {
			BackAction = backAction;
		}

		public string BackAction { get; set; }

	}

}