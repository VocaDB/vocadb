using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class WebLinkInfoViewModel
	{
		public WebLinkInfoViewModel(ArchivedWebLinkContract link)
		{
			Link = link;
		}

		public ArchivedWebLinkContract Link { get; set; }
	}
}