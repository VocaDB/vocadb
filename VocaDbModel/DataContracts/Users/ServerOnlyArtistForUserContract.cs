#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	public class ServerOnlyArtistForUserContract
	{
		public ServerOnlyArtistForUserContract() { }

#nullable enable
		public ServerOnlyArtistForUserContract(OwnedArtistForUser ownedArtistForUser, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => ownedArtistForUser);

			Artist = new ArtistContract(ownedArtistForUser.Artist, languagePreference);
			Id = ownedArtistForUser.Id;
			User = new ServerOnlyUserContract(ownedArtistForUser.User);
		}
#nullable disable

		public ServerOnlyArtistForUserContract(ArtistContract artist)
		{
			Artist = artist;
		}

		public ArtistContract Artist { get; init; }

		public int Id { get; init; }

		public ServerOnlyUserContract User { get; init; }
	}
}
