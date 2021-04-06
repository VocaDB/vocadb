#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists
{
	public class ServerOnlyArtistWithArchivedVersionsContract : ArtistContract
	{
#nullable enable
		public ServerOnlyArtistWithArchivedVersionsContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference)
		{
			ParamIs.NotNull(() => artist);

			ArchivedVersions = artist.ArchivedVersionsManager.Versions.Select(a => new ServerOnlyArchivedArtistVersionContract(a)).ToArray();
		}
#nullable disable

		public ServerOnlyArchivedArtistVersionContract[] ArchivedVersions { get; init; }
	}
}
