using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums;

[DataContract(Namespace = Schemas.VocaDb)]
public class ArchivedAlbumReleaseContract : IAlbumRelease
{
#nullable disable
	public ArchivedAlbumReleaseContract() { }
#nullable enable

	public ArchivedAlbumReleaseContract(AlbumRelease release)
	{
		ParamIs.NotNull(() => release);

		CatNum = release.CatNum;
		ReleaseDate = release.ReleaseDate != null ? new OptionalDateTimeContract(release.ReleaseDate) : null;

		ReleaseEvent = ObjectRefContract.Create(release.ReleaseEvent);
	}

	[DataMember]
	public string? CatNum { get; init; }

	[DataMember]
	public OptionalDateTimeContract? ReleaseDate { get; init; }

	IOptionalDateTime? IAlbumRelease.ReleaseDate => ReleaseDate;

	[DataMember]
	public ObjectRefContract? ReleaseEvent { get; init; }
}
