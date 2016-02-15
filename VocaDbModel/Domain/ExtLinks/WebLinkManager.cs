using System.Collections.Generic;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.ExtLinks {

	public class WebLinkManager<T> where T : WebLink {

		private IList<T> links = new List<T>();

		public virtual IList<T> Links {
			get { return links; }
			set {
				ParamIs.NotNull(() => value);
				links = value;
			}
		}

		public CollectionDiffWithValue<T, T> Sync(IEnumerable<WebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory) {
			return WebLink.Sync(Links, newLinks, webLinkFactory);
		}

		public CollectionDiff<T, T> SyncByValue(IEnumerable<ArchivedWebLinkContract> newLinks, IWebLinkFactory<T> webLinkFactory) {
			return WebLink.SyncByValue(Links, newLinks, webLinkFactory);
		}

	}

}
