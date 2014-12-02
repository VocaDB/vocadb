using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedAlbumReleaseContract : IAlbumRelease {

		public ArchivedAlbumReleaseContract() { }

		public ArchivedAlbumReleaseContract(AlbumRelease release) {

			ParamIs.NotNull(() => release);

			CatNum = release.CatNum;
			ReleaseDate = (release.ReleaseDate != null ? new OptionalDateTimeContract(release.ReleaseDate) : null);

			if (ReleaseDate != null)
				ReleaseDate.Formatted = string.Empty;

			EventName = release.EventName;

		}

		[DataMember]
		public string CatNum { get; set; }

		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; set; }

		IOptionalDateTime IAlbumRelease.ReleaseDate {
			get { return ReleaseDate; }
		}

		[DataMember]
		public string EventName { get; set; }

	}

}
