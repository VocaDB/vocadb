using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ComparedEventsContract : ComparedVersionsContract<ArchivedEventContract> {

		public ComparedEventsContract(ComparedVersionsContract<ArchivedEventContract> comparedVersions)
			: base(comparedVersions) { }

		public static ComparedEventsContract Create(ArchivedReleaseEventVersion firstData, ArchivedReleaseEventVersion secondData) {
			return new ComparedEventsContract(Create(firstData, secondData, ArchivedEventContract.GetAllProperties, d => d.Id));
		}

	}
}
