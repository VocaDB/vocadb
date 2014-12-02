using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumReleaseContract : IAlbumRelease {

		public AlbumReleaseContract() {}

		public AlbumReleaseContract(AlbumRelease release) {
			
			ParamIs.NotNull(() => release);

			CatNum = release.CatNum;
			ReleaseDate = (release.ReleaseDate != null ? new OptionalDateTimeContract(release.ReleaseDate) : null);
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
