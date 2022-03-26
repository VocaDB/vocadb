using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.Users
{
	[DataContract]
	public class ArtistForUserForApiContract
	{
#nullable disable
		public ArtistForUserForApiContract() { }
#nullable enable

		public ArtistForUserForApiContract(
			IArtistLink? artistForUser,
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbPersister,
			ArtistOptionalFields includedFields
		)
		{
			Artist = artistForUser is not null
				? new ArtistForApiContract(artistForUser.Artist, languagePreference, thumbPersister, includedFields)
				: null;
		}

		[DataMember]
		public ArtistForApiContract? Artist { get; init; }
	}
}
