namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class HelpLabelViewModel
	{
		public HelpLabelViewModel(string label, string title, string forElem = null)
		{
			Label = label;
			Title = title;
			ForElem = forElem;
		}

		public string Label { get; set; }

		public string Title { get; set; }

		public string ForElem { get; set; }
	}
}