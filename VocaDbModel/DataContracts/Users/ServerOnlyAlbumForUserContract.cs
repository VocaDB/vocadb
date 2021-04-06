#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	[DataContract(Namespace = Schemas.VocaDb, Name = "AlbumForUserContract")]
	public class ServerOnlyAlbumForUserContract
	{
		public ServerOnlyAlbumForUserContract() { }

#nullable enable
		public ServerOnlyAlbumForUserContract(AlbumForUser albumForUser,
			ContentLanguagePreference languagePreference, bool includeUser = true)
		{
			ParamIs.NotNull(() => albumForUser);

			Album = new AlbumContract(albumForUser.Album, languagePreference);
			Id = albumForUser.Id;
			MediaType = albumForUser.MediaType;
			PurchaseStatus = albumForUser.PurchaseStatus;
			Rating = albumForUser.Rating;

			if (includeUser)
			{
				User = new ServerOnlyUserContract(albumForUser.User);
			}
		}
#nullable disable

		[DataMember]
		public AlbumContract Album { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public MediaType MediaType { get; init; }

		[DataMember]
		public PurchaseStatus PurchaseStatus { get; init; }

		[DataMember]
		public int Rating { get; init; }

		// Note: only needed for album collection. True if public collection or viewer is the user himself.
		public bool ShouldShowCollectionStatus { get; init; }

		/// <summary>
		/// User who rated the album. Can be null for anonymous ratings.
		/// </summary>
		[DataMember]
		public ServerOnlyUserContract User { get; init; }
	}
}
