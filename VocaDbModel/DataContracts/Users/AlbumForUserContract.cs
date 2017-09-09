using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForUserContract {

		public AlbumForUserContract() {}

		public AlbumForUserContract(AlbumForUser albumForUser, 
			ContentLanguagePreference languagePreference, bool includeUser = true) {

			ParamIs.NotNull(() => albumForUser);

			Album = new AlbumContract(albumForUser.Album, languagePreference);
			Id = albumForUser.Id;
			MediaType = albumForUser.MediaType;
			PurchaseStatus = albumForUser.PurchaseStatus;
			Rating = albumForUser.Rating;

			if (includeUser) {
				User = new UserContract(albumForUser.User);
			}

		}

		[DataMember]
		public AlbumContract Album { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public MediaType MediaType { get; set; }

		[DataMember]
		public PurchaseStatus PurchaseStatus { get; set; }

		[DataMember]
		public int Rating { get; set; }

		// Note: only needed for album collection. True if public collection or viewer is the user himself.
		public bool ShouldShowCollectionStatus { get; set; }

		/// <summary>
		/// User who rated the album. Can be null for anonymous ratings.
		/// </summary>
		[DataMember]
		public UserContract User { get; set; }

	}

}
