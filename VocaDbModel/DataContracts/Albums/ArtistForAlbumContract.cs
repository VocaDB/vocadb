#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Model.DataContracts.Albums
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForAlbumContract : IArtistLinkContract
	{
		public ArtistForAlbumContract() { }

#nullable enable
		public ArtistForAlbumContract(ArtistForAlbum artistForAlbum, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => artistForAlbum);

			Artist = (artistForAlbum.Artist != null ? new ArtistContract(artistForAlbum.Artist, languagePreference) : null);
			Categories = artistForAlbum.ArtistCategories;
			EffectiveRoles = artistForAlbum.EffectiveRoles;
			Id = artistForAlbum.Id;
			IsCustomName = !string.IsNullOrEmpty(artistForAlbum.Name);
			IsSupport = artistForAlbum.IsSupport;
			Name = (Artist != null && !IsCustomName ? Artist.Name : artistForAlbum.Name);
			Roles = artistForAlbum.Roles;
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
