using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForUserForApiContract {

		public AlbumForUserForApiContract() { }

		public AlbumForUserForApiContract(
			AlbumForUser albumForUser, 
			ContentLanguagePreference languagePreference, 
			IEntryThumbPersister thumbPersister,
			AlbumOptionalFields fields,
			bool shouldShowCollectionStatus) {

			Album = new AlbumForApiContract(albumForUser.Album, null, languagePreference, thumbPersister, fields, SongOptionalFields.None);
			Rating = albumForUser.Rating;

			if (shouldShowCollectionStatus) {
				MediaType = albumForUser.MediaType;
				PurchaseStatus = albumForUser.PurchaseStatus;
			}

		}


		[DataMember]
		public AlbumForApiContract Album { get; set; }

		/// <summary>
		/// Media type. Can be null if hidden by privacy settings, but otherwise not.
		/// </summary>
		[DataMember]
		public MediaType? MediaType { get; set; }

		/// <summary>
		/// Purchase status. Can be null if hidden privacy setting, but otherwise not.
		/// </summary>
		[DataMember]
		public PurchaseStatus? PurchaseStatus { get; set; }

		/// <summary>
		/// Given rating, generally from 0 to 5.
		/// </summary>
		[DataMember]
		public int Rating { get; set; }

	}

}
