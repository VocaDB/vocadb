using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForSongContract : IArtistLinkContract {

		public ArtistForSongContract() { }

		public ArtistForSongContract(ArtistForSong artistForSong, ContentLanguagePreference languagePreference) {
			
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

		public ArtistForSongContract(ArtistContract artistContract) {

			ParamIs.NotNull(() => artistContract);

			Artist = artistContract;

		}

		public ArtistForSongContract(string name) {

			ParamIs.NotNullOrEmpty(() => name);

			Name = name;

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
		public bool IsCustomName { get; set;}

		[DataMember]
		public bool IsSupport { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public ArtistRoles Roles { get; set; }

	}

}
