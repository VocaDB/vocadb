using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// Data contract for <see cref="User"/>, for details view.
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary>
	public class UserDetailsContract : UserWithPermissionsContract {

		public UserDetailsContract() {}

		public UserDetailsContract(User user, IUserPermissionContext permissionContext) 
			: base(user, permissionContext.LanguagePreference, getPublicCollection: true) {

			AboutMe = user.Options.AboutMe;
			EmailVerified = user.Options.EmailVerified;
			LastLogin = user.LastLogin;
			LastLoginAddress = user.Options.LastLoginAddress;
			Location = user.Options.Location;
			TwitterName = user.Options.TwitterName;
			WebLinks = user.WebLinks.OrderBy(w => w.DescriptionOrUrl).Select(w => new WebLinkContract(w)).ToArray();

		}

		public string AboutMe { get; set; }

		public int AlbumCollectionCount { get; set; }

		public int ArtistCount { get; set; }

		public int CommentCount { get; set; }

		public bool EmailVerified { get; set; }

		public int EditCount { get; set; }

		public AlbumContract[] FavoriteAlbums { get; set;}

		public int FavoriteSongCount { get; set; }

		public string[] FavoriteTags { get; set;}

		public ArtistContract[] FollowedArtists { get; set;}

		public DateTime LastLogin { get; set; }

		public string LastLoginAddress { get; set; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

		public SongContract[] LatestRatedSongs { get; set; }

		public int Level { get; set; }

		public string Location { get; set; }

		public int Power { get; set; }

		/// <summary>
		/// User is a possible producer account, not yet verified.
		/// This is done by matching username with the artist name.
		/// </summary>
		public bool PossibleProducerAccount { get; set; }

		public SongListContract[] SongLists { get; set; }

		public int SubmitCount { get; set; }

		public int TagVotes { get; set; }

		public string TwitterName { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}
}
