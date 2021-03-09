#nullable disable

using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	public class ArtistForUserContract
	{
		public ArtistForUserContract() { }

		public ArtistForUserContract(OwnedArtistForUser ownedArtistForUser, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => ownedArtistForUser);

			Artist = new ArtistContract(ownedArtistForUser.Artist, languagePreference);
			Id = ownedArtistForUser.Id;
			User = new UserContract(ownedArtistForUser.User);
		}

		public ArtistForUserContract(ArtistContract artist)
		{
			Artist = artist;
		}

		public ArtistContract Artist { get; init; }

		public int Id { get; init; }

		public UserContract User { get; init; }
	}
}
