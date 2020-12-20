#nullable disable

using System.Collections.Generic;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Web.Models.Shared.Partials.EntryDetails
{
	public class ExternalLinksListViewModel
	{
		public ExternalLinksListViewModel(IEnumerable<IWebLinkWithDescriptionOrUrl> webLinks, bool showCategory = false)
		{
			WebLinks = webLinks;
			ShowCategory = showCategory;
		}

		public IEnumerable<IWebLinkWithDescriptionOrUrl> WebLinks { get; set; }
		
		public bool ShowCategory { get; set; }
	}
}