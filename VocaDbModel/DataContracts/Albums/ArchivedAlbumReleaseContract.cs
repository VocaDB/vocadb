#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedAlbumReleaseContract : IAlbumRelease
	{
		public ArchivedAlbumReleaseContract() { }

#nullable enable
		public ArchivedAlbumReleaseContract(AlbumRelease release)
		{
			ParamIs.NotNull(() => release);

			CatNum = release.CatNum;
			ReleaseDate = (release.ReleaseDate != null ? new OptionalDateTimeContract(release.ReleaseDate) : null);

			if (ReleaseDate != null)
				ReleaseDate.Formatted = string.Empty;

			ReleaseEvent = ObjectRefContract.Create(release.ReleaseEvent);
		}
#nullable disable

		[DataMember]
		public string CatNum { get; init; }

		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; init; }

		IOptionalDateTime IAlbumRelease.ReleaseDate => ReleaseDate;

		[DataMember]
		public ObjectRefContract ReleaseEvent { get; init; }
	}
}
