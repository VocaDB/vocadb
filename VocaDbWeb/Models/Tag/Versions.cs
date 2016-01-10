using System.Linq;
using VocaDb.Model;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Tags;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Tag {

	public class Versions {

		public static ArchivedObjectVersion CreateForTag(ArchivedTagVersionContract tag) {

			return new ArchivedObjectVersion(tag, GetReasonName(tag.Reason, tag.Notes),
				GetChangeString(tag.ChangedFields), tag.Reason != EntryEditEvent.Updated || tag.ChangedFields != TagEditableFields.Nothing);

		}

		public static string GetChangeString(TagEditableFields fields) {

			if (fields == TagEditableFields.Nothing)
				return string.Empty;

			return Translate.TagEditableFieldNames.GetAllNameNames(fields, TagEditableFields.Nothing);

		}

		private static string GetReasonName(EntryEditEvent reason, string notes) {

			return Translate.EntryEditEventNames[reason];

		}

		public Versions() { }

		public Versions(TagWithArchivedVersionsContract contract) {

			ParamIs.NotNull(() => contract);

			Tag = contract;
			ArchivedVersions = contract.ArchivedVersions.Select(CreateForTag).ToArray();

		}

		public ArchivedObjectVersion[] ArchivedVersions { get; set; }

		public TagContract Tag { get; set; }

	}

}