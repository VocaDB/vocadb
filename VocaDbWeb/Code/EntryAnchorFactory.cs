using System.Web;
using VocaDb.Model;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code {

	public class EntryAnchorFactory : IEntryLinkFactory {

		private readonly string baseUrl;
		private readonly string hostAddress;

		private string CreateAnchor(string href, string text) {
			return string.Format("<a href=\"{0}\">{1}</a>", href, HttpUtility.HtmlEncode(text));
		}

		/// <summary>
		/// Initializes entry anchor factory.
		/// </summary>
		/// <param name="hostAddress">Application host address, for example http://vocadb.net/ </param>
		/// <param name="baseUrl">
		/// Base URL to the website. This will be added before the relative URL. Cannot be null. Can be empty.
		/// </param>
		public EntryAnchorFactory(string hostAddress, string baseUrl = "/") {

			ParamIs.NotNull(() => baseUrl);

			this.hostAddress = hostAddress;
			this.baseUrl = baseUrl;

		}

		private string GetUrl(string basePart, EntryType entryType, int id, string slug) {

			string relative;
			slug = slug ?? string.Empty;

			switch (entryType) {
				case EntryType.Album:
					relative = string.Format("Al/{0}", id);
					break;

				case EntryType.Artist:
					relative = string.Format("Ar/{0}", id);
					break;

				case EntryType.DiscussionTopic:
					relative = string.Format("discussion/topics/{0}", id);
					break;

				case EntryType.Song:
					relative = string.Format("S/{0}", id);
					break;

				case EntryType.Tag:
					relative = string.Format("T/{0}{1}{2}", id, slug != string.Empty ? "/" : string.Empty, slug);
					break;

				default:
					relative = string.Format("{0}/Details/{1}", entryType, id);	
					break;
			}

			return VocaUriBuilder.MergeUrls(basePart, relative);

		}

		public string GetFullEntryUrl(EntryType entryType, int id, string slug = null) {
			return GetUrl(hostAddress, entryType, id, slug);
		}

		public string CreateEntryLink(EntryType entryType, int id, string name, string slug = null) {

			var url = GetUrl(baseUrl, entryType, id, slug);

			return CreateAnchor(url, name);

		}

		public string CreateEntryLink(IEntryBase entry, string slug = null) {

			return CreateEntryLink(entry.EntryType, entry.Id, entry.DefaultName, slug);

		}

	}
}