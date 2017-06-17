using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ArchivedEventVersionDetailsContract {

		public ArchivedEventVersionDetailsContract() { }

		public ArchivedEventVersionDetailsContract(ArchivedReleaseEventVersion archived, ArchivedReleaseEventVersion comparedVersion, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedEventVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedEventVersionContract(comparedVersion) : null;
			ComparedVersionId = comparedVersion != null ? comparedVersion.Id : 0;
			ReleaseEvent = new ReleaseEventContract(archived.ReleaseEvent, languagePreference);
			Name = ReleaseEvent.Name;

			ComparableVersions = archived.ReleaseEvent.ArchivedVersionsManager
				.GetPreviousVersions(archived)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedEventsContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;

		}

		public ArchivedObjectVersionContract ArchivedVersion { get; set; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; set; }

		public ArchivedObjectVersionContract ComparedVersion { get; set; }

		public int ComparedVersionId { get; set; }

		public ReleaseEventContract ReleaseEvent { get; set; }

		public string Name { get; set; }

		public ComparedEventsContract Versions { get; set; }

	}
}
