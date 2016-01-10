using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class ComparedTagsContract : ComparedVersionsContract<ArchivedTagContract> {

		public ComparedTagsContract(ComparedVersionsContract<ArchivedTagContract> comparedVersions)
			: base(comparedVersions) { }

		public static ComparedTagsContract Create(ArchivedTagVersion firstData, ArchivedTagVersion secondData) {
			return new ComparedTagsContract(Create(firstData, secondData, ArchivedTagContract.GetAllProperties, d => d.Id));
		}

	}

}
