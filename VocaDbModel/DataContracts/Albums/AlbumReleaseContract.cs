#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Albums
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumReleaseContract : IAlbumRelease
	{
		public AlbumReleaseContract() { }

		public AlbumReleaseContract(AlbumRelease release, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => release);

			CatNum = release.CatNum;
			ReleaseDate = (release.ReleaseDate != null ? new OptionalDateTimeContract(release.ReleaseDate) : null);
			ReleaseEvent = release.ReleaseEvent != null ? new ReleaseEventForApiContract(release.ReleaseEvent, languagePreference, ReleaseEventOptionalFields.None, null) : null;
		}

		[DataMember]
		public string CatNum { get; init; }

		[DataMember]
		public OptionalDateTimeContract ReleaseDate { get; init; }

		IOptionalDateTime IAlbumRelease.ReleaseDate => ReleaseDate;

		[DataMember]
		public ReleaseEventForApiContract ReleaseEvent { get; init; }
	}
}
