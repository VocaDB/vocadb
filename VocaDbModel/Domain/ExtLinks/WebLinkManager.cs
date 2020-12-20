#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ExtLinks
{
	public class WebLinkManager<T> where T : WebLink
	{
		private IList<T> _links = new List<T>();

		public virtual bool HasLink(string url)
		{
			return Links.Any(l => l.Url == url);
		}

		public virtual IList<T> Links
		{
			get => _links;
			set
			{
				ParamIs.NotNull(() => value);
				_links = value;
			}
		}

		public CollectionDiffWithValue<T, T> Sync(IEnumerable<WebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory)
		{
			return WebLink.Sync(Links, newLinks, webLinkFactory);
		}

		public CollectionDiff<T, T> SyncByValue(IEnumerable<ArchivedWebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory)
		{
			return WebLink.SyncByValue(Links, newLinks, webLinkFactory);
		}
	}
}
