#nullable disable

using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.DataContracts.Venues
{
	public class ComparedVenueContract : ComparedVersionsContract<ArchivedVenueContract>
	{
		public ComparedVenueContract(ComparedVersionsContract<ArchivedVenueContract> comparedVersions) : base(comparedVersions) { }

		public static ComparedVenueContract Create(ArchivedVenueVersion firstData, ArchivedVenueVersion secondData)
		{
			return new ComparedVenueContract(Create(firstData, secondData, ArchivedVenueContract.GetAllProperties, d => d.Id));
		}
	}
}
