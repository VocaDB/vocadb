using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.ReleaseEvents;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumReleaseContract : IAlbumRelease {

		public AlbumReleaseContract() {}

		public AlbumReleaseContract(AlbumRelease release) {
			
			ParamIs.NotNull(() => release);

			CatNum = release.CatNum;
			ReleaseDate = (release.ReleaseDate != null ? new OptionalDateTimeContract(release.ReleaseDate) : null);
			ReleaseEvent = release.ReleaseEvent != null ? new ReleaseEventForApiContract(release.ReleaseEvent, ReleaseEventOptionalFields.None) : null;

		}

		[DataMember]
		public string CatNum { get; set; }

		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; set; }

		IOptionalDateTime IAlbumRelease.ReleaseDate => ReleaseDate;

		[DataMember]
		public ReleaseEventForApiContract ReleaseEvent { get; set; }

	}

}
