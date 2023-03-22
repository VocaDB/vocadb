using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs;

[DataContract(Namespace = Schemas.VocaDb)]
public class SongInAlbumForApiContract
{
#nullable disable
	public SongInAlbumForApiContract() { }
#nullable enable

	public SongInAlbumForApiContract(
		SongInAlbum songInAlbum,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		SongOptionalFields fields,
		SongVoteRating? rating = null
	)
	{
		ParamIs.NotNull(() => songInAlbum);

		DiscNumber = songInAlbum.DiscNumber;
		Id = songInAlbum.Id;
		TrackNumber = songInAlbum.TrackNumber;

		var song = songInAlbum.Song;

		Song = song is not null
			? new SongForApiContract(
				song: song,
				mergeRecord: null,
				languagePreference: languagePreference,
				permissionContext,
				fields: fields
			)
			: null;

		Name = Song is not null
			? Song.Name
			: songInAlbum.Name;

		Rating = rating;
	}

	[DataMember]
	public int DiscNumber { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public SongVoteRating? Rating { get; init; }

	[DataMember]
	public SongForApiContract? Song { get; init; }

	[DataMember]
	public int TrackNumber { get; init; }
}
