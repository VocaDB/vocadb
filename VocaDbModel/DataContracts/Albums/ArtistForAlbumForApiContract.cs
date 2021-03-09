#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForAlbumForApiContract
	{
		public ArtistForAlbumForApiContract() { }

		public ArtistForAlbumForApiContract(ArtistForAlbum artistForAlbum, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => artistForAlbum);

			Artist = (artistForAlbum.Artist != null ? new ArtistContract(artistForAlbum.Artist, languagePreference) : null);
			Categories = artistForAlbum.ArtistCategories;
			EffectiveRoles = artistForAlbum.EffectiveRoles;
			IsSupport = artistForAlbum.IsSupport;
			Name = (Artist != null ? Artist.Name : artistForAlbum.Name);
			Roles = artistForAlbum.Roles;
		}

		[DataMember]
		public ArtistContract Artist { get; init; }

		[DataMember]
		public ArtistCategories Categories { get; init; }

		[DataMember]
		public ArtistRoles EffectiveRoles { get; init; }

		[DataMember]
		public bool IsSupport { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public ArtistRoles Roles { get; init; }
	}
}
