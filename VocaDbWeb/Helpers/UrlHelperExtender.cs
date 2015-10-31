using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Helpers {

	public static class UrlHelperExtender {

		public static string EntryDetails(this UrlHelper urlHelper, IEntryBase entryBase) {
			
			ParamIs.NotNull(() => entryBase);

			switch (entryBase.EntryType) {
				case EntryType.ReleaseEvent:
					return urlHelper.Action("Details", "Event", new { id = entryBase.Id });

				case EntryType.Tag:
					return urlHelper.Action("DetailsById", "Tag", new { id = entryBase.Id });

				default:
					return urlHelper.Action("Details", entryBase.EntryType.ToString(), new { id = entryBase.Id });
			}

		}

		public static string EntryDetails(this UrlHelper urlHelper, EntryForApiContract entry) {

			ParamIs.NotNull(() => entry);

			switch (entry.EntryType) {
				case EntryType.ReleaseEvent:
					return urlHelper.Action("Details", "Event", new { id = entry.Id });

				case EntryType.Tag:
					return urlHelper.Action("DetailsById", "Tag", new { id = entry.Id, slug = entry.UrlSlug });

				default:
					return urlHelper.Action("Details", entry.EntryType.ToString(), new { id = entry.Id });
			}

		}

		public static string UserDetails(this UrlHelper urlHelper, IUser user) {

			ParamIs.NotNull(() => user);

			return urlHelper.Action("Details", "User", new { id = user.Name });

		}

	}

}