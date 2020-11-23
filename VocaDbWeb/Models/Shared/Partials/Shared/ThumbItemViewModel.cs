using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{

	public class ThumbItemViewModel
	{

		public ThumbItemViewModel(string url, string thumbUrl, string caption, IEntryBase entry = null)
		{
			Url = url;
			ThumbUrl = thumbUrl;
			Caption = caption;
			Entry = entry;
		}

		public string Url { get; set; }

		public string ThumbUrl { get; set; }

		public string Caption { get; set; }

		public IEntryBase Entry { get; set; }

	}

}