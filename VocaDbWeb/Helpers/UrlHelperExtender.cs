using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.Domain;

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

	}

}