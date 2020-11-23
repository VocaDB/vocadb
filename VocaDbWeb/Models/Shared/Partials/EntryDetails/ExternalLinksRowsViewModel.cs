using System.Collections.Generic;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.EntryDetails
{
	public class ExternalLinksRowsViewModel
	{
		public ExternalLinksRowsViewModel(ICollection<WebLinkContract> webLinks)
		{
			WebLinks = webLinks;
		}

		public ICollection<WebLinkContract> WebLinks { get; set; }
	}
}