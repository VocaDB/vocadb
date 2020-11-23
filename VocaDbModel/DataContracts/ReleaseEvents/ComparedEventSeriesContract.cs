using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{

	public class ComparedEventSeriesContract : ComparedVersionsContract<ArchivedEventSeriesContract>
	{

		public ComparedEventSeriesContract(ComparedVersionsContract<ArchivedEventSeriesContract> comparedVersions)
			: base(comparedVersions) { }

		public static ComparedEventSeriesContract Create(ArchivedReleaseEventSeriesVersion firstData, ArchivedReleaseEventSeriesVersion secondData)
		{
			return new ComparedEventSeriesContract(Create(firstData, secondData, ArchivedEventSeriesContract.GetAllProperties, d => d.Id));
		}

	}

}
