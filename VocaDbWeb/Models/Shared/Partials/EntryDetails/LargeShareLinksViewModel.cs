namespace VocaDb.Web.Models.Shared.Partials.EntryDetails
{

	public class LargeShareLinksViewModel
	{

		public LargeShareLinksViewModel(string title, string url)
		{
			Title = title;
			Url = url;
		}

		public string Title { get; set; }

		public string Url { get; set; }

	}

}