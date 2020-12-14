#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class PictureRowViewModel
	{
		public PictureRowViewModel(string name, string url, string compareToUrl = null)
		{
			Name = name;
			Url = url;
			CompareToUrl = compareToUrl;
		}

		public string Name { get; set; }

		public string Url { get; set; }

		public string CompareToUrl { get; set; }
	}
}