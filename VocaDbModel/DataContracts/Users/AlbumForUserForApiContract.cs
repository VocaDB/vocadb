#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumForUserForApiContract
	{
		public AlbumForUserForApiContract() { }

		public AlbumForUserForApiContract(
			AlbumForUser albumForUser,
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbPersister,
			AlbumOptionalFields fields,
			bool shouldShowCollectionStatus,
			bool includeUser = false)
		{
			Album = albumForUser != null ? new AlbumForApiContract(albumForUser.Album, null, languagePreference, thumbPersister, fields, SongOptionalFields.None) : null;
			Rating = albumForUser?.Rating ?? 0;

			if (shouldShowCollectionStatus)
			{
				MediaType = albumForUser?.MediaType ?? null;
				PurchaseStatus = albumForUser?.PurchaseStatus ?? null;
			}

			if (includeUser)
			{
				User = albumForUser != null ? new UserForApiContract(albumForUser.User) : null;
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

		[DataMember]
		public UserForApiContract User { get; set; }
	}
}
