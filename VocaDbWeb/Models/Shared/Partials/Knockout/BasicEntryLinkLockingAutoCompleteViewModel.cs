namespace VocaDb.Web.Models.Shared.Partials.Knockout {

	public class BasicEntryLinkLockingAutoCompleteViewModel {

		public BasicEntryLinkLockingAutoCompleteViewModel(string bindingHandler, string binding, string extraBindings = null) {
			BindingHandler = bindingHandler;
			Binding = binding;
			ExtraBindings = extraBindings;
		}

		public string BindingHandler { get; set; }

		public string Binding { get; set; }

		public string ExtraBindings { get; set; }

	}

}