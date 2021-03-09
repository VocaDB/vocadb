#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Data contract for <see cref="User"/>, for details view.
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary>
	public class UserDetailsContract : UserWithPermissionsContract
	{
		public UserDetailsContract() { }

		public UserDetailsContract(User user, IUserPermissionContext permissionContext)
			: base(user, permissionContext.LanguagePreference, getPublicCollection: true)
		{
			AboutMe = user.Options.AboutMe;
			CustomTitle = user.Options.CustomTitle;
			EmailVerified = user.Options.EmailVerified;
			LastLogin = user.LastLogin;
			LastLoginAddress = user.Options.LastLoginAddress;
			Location = user.Options.Location;
			KnownLanguages = user.KnownLanguages.OrderByDescending(l => l.Proficiency).Select(l => new UserKnownLanguageContract(l)).ToArray();
			OldUsernames = user.OldUsernames.Select(n => new OldUsernameContract(n)).ToArray();
			Standalone = user.Options.Standalone;
			TwitterName = user.Options.TwitterName;
			WebLinks = user.WebLinks.OrderBy(w => w.DescriptionOrUrl).Select(w => new WebLinkContract(w)).ToArray();
		}

		public string AboutMe { get; init; }

		public int AlbumCollectionCount { get; set; }

		public int ArtistCount { get; set; }

		public int CommentCount { get; set; }

		public string CustomTitle { get; init; }

		public bool EmailVerified { get; init; }

		public int EditCount { get; set; }

		public AlbumForApiContract[] FavoriteAlbums { get; set; }

		public int FavoriteSongCount { get; set; }

		public TagBaseContract[] FavoriteTags { get; set; }

		public ArtistContract[] FollowedArtists { get; set; }

		public bool IsVeteran { get; set; }

		public DateTime LastLogin { get; init; }

		public string LastLoginAddress { get; init; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

		public SongForApiContract[] LatestRatedSongs { get; set; }

		public int Level { get; set; }

		public string Location { get; init; }

		public UserKnownLanguageContract[] KnownLanguages { get; init; }

		public OldUsernameContract[] OldUsernames { get; init; }

		public int Power { get; set; }

		/// <summary>
		/// User is a possible producer account, not yet verified.
		/// This is done by matching username with the artist name.
		/// </summary>
		public bool PossibleProducerAccount { get; set; }

		public SongListContract[] SongLists { get; init; }

		public bool Standalone { get; init; }

		public int SubmitCount { get; set; }

		public int TagVotes { get; set; }

		public string TwitterName { get; init; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; init; }
	}
}
