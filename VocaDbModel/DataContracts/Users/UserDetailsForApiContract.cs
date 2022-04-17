using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	/// <summary>
	/// Contains no sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public abstract record UserDetailsForApiContractBase
	{
		[DataMember]
		public string AboutMe { get; init; }

		[DataMember]
		public bool Active { get; init; }

		[DataMember]
		public int AlbumCollectionCount { get; set; }

		[DataMember]
		public bool AnonymousActivity { get; init; }

		[DataMember]
		public int ArtistCount { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public DateTime CreateDate { get; init; }

		[DataMember]
		public string CustomTitle { get; init; }

		[DataMember]
		public bool DesignatedStaff { get; init; }

		[DataMember]
		public int EditCount { get; set; }

		[DataMember]
		public bool EmailVerified { get; init; }

		[DataMember]
		public AlbumForApiContract[] FavoriteAlbums { get; set; } = default!;

		[DataMember]
		public int FavoriteSongCount { get; set; }

		[DataMember]
		public TagBaseContract[] FavoriteTags { get; set; } = default!;

		[DataMember]
		public ArtistForApiContract[] FollowedArtists { get; set; } = default!;

		[DataMember]
		public UserGroupId GroupId { get; init; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public bool IsVeteran { get; set; }

		[DataMember]
		public UserKnownLanguageContract[] KnownLanguages { get; init; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; } = default!;

		[DataMember]
		public SongForApiContract[] LatestRatedSongs { get; set; } = default!;

		[DataMember]
		public int Level { get; set; }

		[DataMember]
		public string Location { get; init; }

		/// <summary>
		/// Can be null.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; init; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public OldUsernameContract[] OldUsernames { get; init; }

		/// <summary>
		/// List of artist entries owned by the user. Cannot be null.
		/// </summary>
		[DataMember]
		public ArtistForUserForApiContract[] OwnedArtistEntries { get; init; }

		/// <summary>
		/// User is a possible producer account, not yet verified.
		/// This is done by matching username with the artist name.
		/// </summary>
		[DataMember]
		public bool PossibleProducerAccount { get; set; }

		[DataMember]
		public int Power { get; set; }

		[DataMember]
		public bool PublicAlbumCollection { get; init; }

		[DataMember]
		public bool Standalone { get; init; }

		[DataMember]
		public int SubmitCount { get; set; }

		[DataMember]
		public bool Supporter { get; init; }

		[DataMember]
		public int TagVotes { get; set; }

		[DataMember]
		public string TwitterName { get; init; }

		[DataMember]
		public bool VerifiedArtist { get; init; }

		[DataMember]
		public WebLinkForApiContract[] WebLinks { get; init; }

		protected UserDetailsForApiContractBase(
			User user,
			IUserIconFactory iconFactory,
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbPersister
		)
		{
			AboutMe = user.Options.AboutMe;
			Active = user.Active;
			AnonymousActivity = user.AnonymousActivity;
			CreateDate = user.CreateDate;
			CustomTitle = user.Options.CustomTitle;
			DesignatedStaff = user.EffectivePermissions.Contains(PermissionToken.DesignatedStaff);
			EmailVerified = user.Options.EmailVerified;
			GroupId = user.GroupId;
			Id = user.Id;

			KnownLanguages = user.KnownLanguages
				.OrderByDescending(l => l.Proficiency)
				.Select(l => new UserKnownLanguageContract(l))
				.ToArray();

			Location = user.Options.Location;
			MainPicture = iconFactory.GetIcons(user, ImageSizes.All);
			Name = user.Name;
			OldUsernames = user.OldUsernames.Select(n => new OldUsernameContract(n)).ToArray();

			OwnedArtistEntries = user.OwnedArtists
				.Select(a => new ArtistForUserForApiContract(
					artistForUser: a,
					languagePreference: languagePreference,
					thumbPersister: thumbPersister,
					includedFields: ArtistOptionalFields.AdditionalNames | ArtistOptionalFields.MainPicture
				))
				.ToArray();

			PublicAlbumCollection = user.Options.PublicAlbumCollection;
			Standalone = user.Options.Standalone;
			Supporter = user.Options.Supporter;
			TwitterName = user.Options.TwitterName;
			VerifiedArtist = user.VerifiedArtist;

			WebLinks = user.WebLinks
				.OrderBy(w => w.DescriptionOrUrl)
				.Select(w => new WebLinkForApiContract(w))
				.ToArray();
		}
	}

	/// <summary>
	/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record UserDetailsForApiContract : UserDetailsForApiContractBase
	{
		[DataMember]
		public Guid[]? AdditionalPermissions { get; init; }

		[DataMember]
		public Guid[]? EffectivePermissions { get; init; }

		[DataMember]
		public string? Email { get; init; }

		[DataMember]
		public DateTime? LastLogin { get; init; }

		[DataMember]
		public string? LastLoginAddress { get; init; }

		public UserDetailsForApiContract(
			User user,
			IUserIconFactory iconFactory,
			ContentLanguagePreference languagePreference,
			IAggregatedEntryImageUrlFactory thumbPersister,
			IUserPermissionContext permissionContext
		)
			: base(user, iconFactory, languagePreference, thumbPersister)
		{
			if (permissionContext.HasPermission(PermissionToken.ManageUserPermissions))
			{
				AdditionalPermissions = user.AdditionalPermissions.PermissionTokens.Select(p => p.Id).ToArray();
				EffectivePermissions = user.EffectivePermissions.PermissionTokens.Select(p => p.Id).ToArray();
				Email = user.Email;
				LastLogin = user.LastLogin;
				LastLoginAddress = user.Options.LastLoginAddress;
			}
			else
			{
				// SECURITY NOTE: Never include sensitive information here!

				if (permissionContext.IsLoggedIn && permissionContext.LoggedUser.Id == user.Id && permissionContext.LoggedUser.Active)
					AdditionalPermissions = user.AdditionalPermissions.PermissionTokens.Select(p => p.Id).ToArray();
			}
		}
	}
}
