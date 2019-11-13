using System.Linq;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ArchivedEventSeriesVersionDetailsContract {

		public ArchivedEventSeriesVersionDetailsContract(ArchivedReleaseEventSeriesVersion archived, ArchivedReleaseEventSeriesVersion comparedVersion, ContentLanguagePreference languagePreference) {

			ParamIs.NotNull(() => archived);

			ArchivedVersion = new ArchivedEventSeriesVersionContract(archived);
			ComparedVersion = comparedVersion != null ? new ArchivedEventSeriesVersionContract(comparedVersion) : null;
			ReleaseEventSeries = new ReleaseEventSeriesContract(archived.Entry, languagePreference);
			Name = ReleaseEventSeries.Name;

			ComparableVersions = archived.Entry.ArchivedVersionsManager
				.GetPreviousVersions(archived)
				.Select(a => ArchivedObjectVersionWithFieldsContract.Create(a, a.Diff.ChangedFields.Value, a.CommonEditEvent))
				.ToArray();

			Versions = ComparedEventSeriesContract.Create(archived, comparedVersion);

			ComparedVersionId = Versions.SecondId;

		}

		public ArchivedEventSeriesVersionContract ArchivedVersion { get; set; }

		public ArchivedObjectVersionContract[] ComparableVersions { get; set; }

		public ArchivedObjectVersionContract ComparedVersion { get; set; }

		public int ComparedVersionId { get; set; }

		public ReleaseEventSeriesContract ReleaseEventSeries { get; set; }

		public string Name { get; set; }

		public ComparedEventSeriesContract Versions { get; set; }

	}

}
