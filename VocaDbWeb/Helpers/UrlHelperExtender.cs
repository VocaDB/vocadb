using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Helpers {

	public static class UrlHelperExtender {

		private static string EntryDetails(UrlHelper urlHelper, EntryType entryType, int id, string urlSlug) {

			switch (entryType) {
				case EntryType.DiscussionTopic:
					return urlHelper.Action("Index", "Discussion", new { clientPath = string.Format("topics/{0}", id) });

				case EntryType.ReleaseEvent:
					return urlHelper.Action("Details", "Event", new { id, slug = urlSlug });

				case EntryType.Tag:
					return urlHelper.Action("DetailsById", "Tag", new { id, slug = urlSlug });

				default:
					return urlHelper.Action("Details", entryType.ToString(), new { id });
			}

		}

		public static string EntryDetails(this UrlHelper urlHelper, IEntryBase entryBase) {
			
			ParamIs.NotNull(() => entryBase);

			return EntryDetails(urlHelper, entryBase.EntryType, entryBase.Id, null);

		}

		public static string EntryDetails(this UrlHelper urlHelper, EntryForApiContract entry) {

			ParamIs.NotNull(() => entry);

			return EntryDetails(urlHelper, entry.EntryType, entry.Id, entry.UrlSlug);

		}

		public static string SongDetails(this UrlHelper urlHelper, IEntryBase song, int? albumId = null) {

			ParamIs.NotNull(() => song);

			return urlHelper.Action("Details", "Song", new { id = song.Id, albumId });

		}

		public static string StaticResource(this UrlHelper urlHelper, string url) {
			return VocaUriBuilder.StaticResource(url);
		}

		public static string UserDetails(this UrlHelper urlHelper, IUser user) {

			ParamIs.NotNull(() => user);

			return urlHelper.Action("Profile", "User", new { id = user.Name });

		}

	}

}