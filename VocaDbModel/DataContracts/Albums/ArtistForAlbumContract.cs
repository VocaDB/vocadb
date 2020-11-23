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

		[DataMember]
		public ArtistContract Artist { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistCategories Categories { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistRoles EffectiveRoles { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public bool IsCustomName { get; set; }

		[DataMember]
		public bool IsSupport { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistRoles Roles { get; set; }

	}

}
