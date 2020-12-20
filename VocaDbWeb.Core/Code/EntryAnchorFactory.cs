#nullable disable

using System.Web;
using VocaDb.Model;
using VocaDb.Model.Domain;
using VocaDb.Model.Service;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code
{
	public class EntryAnchorFactory : IEntryLinkFactory
	{
		private readonly string _baseUrl;
		private readonly string _hostAddress;

		private string CreateAnchor(string href, string text)
		{
			return $"<a href=\"{href}\">{HttpUtility.HtmlEncode(text)}</a>";
		}

		/// <summary>
		/// Initializes entry anchor factory.
		/// </summary>
		/// <param name="hostAddress">Application host address, for example http://vocadb.net/ </param>
		/// <param name="baseUrl">
		/// Base URL to the website. This will be added before the relative URL. Cannot be null. Can be empty.
		/// </param>
		public EntryAnchorFactory(string hostAddress, string baseUrl = "/")
		{
			ParamIs.NotNull(() => baseUrl);

			_hostAddress = hostAddress;
			_baseUrl = baseUrl;
		}

		private string GetUrl(string basePart, EntryType entryType, int id, string slug)
		{
			slug ??= string.Empty;

			var slashForSlug = slug != string.Empty ? "/" : string.Empty;
			var relative = entryType switch
			{
				EntryType.Album => $"Al/{id}",
				EntryType.Artist => $"Ar/{id}",
				EntryType.DiscussionTopic => $"discussion/topics/{id}",
				EntryType.ReleaseEvent => $"E/{id}{slashForSlug}{slug}",
				EntryType.ReleaseEventSeries => $"Event/SeriesDetails/{id}",
				EntryType.Song => $"S/{id}",
				EntryType.Tag => $"T/{id}{slashForSlug}{slug}",
				_ => $"{entryType}/Details/{id}",
			};
			return VocaUriBuilder.MergeUrls(basePart, relative);
		}

		public string GetFullEntryUrl(EntryType entryType, int id, string slug = null)
		{
			return GetUrl(_hostAddress, entryType, id, slug);
		}

		public string CreateEntryLink(EntryType entryType, int id, string name, string slug = null)
		{
			var url = GetUrl(_baseUrl, entryType, id, slug);

			return CreateAnchor(url, name);
		}

		public string CreateEntryLink(IEntryBase entry, string slug = null)
		{
			return CreateEntryLink(entry.EntryType, entry.Id, entry.DefaultName, slug);
		}
	}
}