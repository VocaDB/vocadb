using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class LockingAutoCompleteViewModel
	{
		public LockingAutoCompleteViewModel(string bindingHandler, string binding, string textBinding = null, string valBinding = null, string extraBindings = null, EntryType entryType = EntryType.Undefined)
		{
			BindingHandler = bindingHandler;
			Binding = binding;
			TextBinding = textBinding;
			ValBinding = valBinding;
			ExtraBindings = extraBindings;
			EntryType = entryType;
		}

		public string BindingHandler { get; set; }

		public string Binding { get; set; }

		public string TextBinding { get; set; }

		public string ValBinding { get; set; }

		public string ExtraBindings { get; set; }

		public EntryType EntryType { get; set; }
	}
}