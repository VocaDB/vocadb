#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForSongContract : IArtistLinkContract
	{
		public ArtistForSongContract() { }

#nullable enable
		public ArtistForSongContract(ArtistForSong artistForSong, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => artistForSong);

			Artist = (artistForSong.Artist != null ? new ArtistContract(artistForSong.Artist, languagePreference) : null);
			Categories = artistForSong.ArtistCategories;
			EffectiveRoles = artistForSong.EffectiveRoles;
			Id = artistForSong.Id;
			IsCustomName = !string.IsNullOrEmpty(artistForSong.Name);
			IsSupport = artistForSong.IsSupport;
			Name = (Artist != null && !IsCustomName ? Artist.Name : artistForSong.Name);
			Roles = artistForSong.Roles;
		}

		public ArtistForSongContract(ArtistContract artistContract)
		{
			ParamIs.NotNull(() => artistContract);

			Artist = artistContract;
		}

		public ArtistForSongContract(string name)
		{
			ParamIs.NotNullOrEmpty(() => name);

			Name = name;
		}
#nullable disable

		[DataMember]
		public ArtistContract Artist { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistCategories Categories { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistRoles EffectiveRoles { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public bool IsCustomName { get; init; }

		[DataMember]
		public bool IsSupport { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistRoles Roles { get; init; }
	}
}
