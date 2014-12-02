using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract]
	public class ArtistForUserForApiContract {

		public ArtistForUserForApiContract() {
			
		}

		public ArtistForUserForApiContract(ArtistForUser artistForUser, 
			ContentLanguagePreference languagePreference, 
			IEntryThumbPersister thumbPersister,
			bool ssl,
			ArtistOptionalFields includedFields) {

			Artist = new ArtistForApiContract(artistForUser.Artist, languagePreference, thumbPersister, ssl, includedFields);

		}

		[DataMember]
		public ArtistForApiContract Artist { get; set; }

	}

}
