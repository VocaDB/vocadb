namespace VocaDb.Web.Models.Shared.Partials.Html
{
	public class FormatMarkdownViewModel
	{
		public FormatMarkdownViewModel(string text)
		{
			Text = text;
		}

		public string Text { get; set; }
	}
}