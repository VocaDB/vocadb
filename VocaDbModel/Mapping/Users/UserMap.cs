using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users;

public class UserMap : ClassMap<User>
{
	public UserMap()
	{
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.AccessKey).Length(20).Not.Nullable();
		Map(m => m.Active).Not.Nullable();
		Map(m => m.AnonymousActivity).Not.Nullable();
		Map(m => m.CreateDate).Not.Nullable();
		Map(m => m.Culture).Not.Nullable();
		Map(m => m.DefaultLanguageSelection).Not.Nullable();
		Map(m => m.Email).Length(50).Not.Nullable();
		Map(m => m.EmailOptions).CustomType(typeof(UserEmailOptions)).Not.Nullable();
		Map(m => m.GroupId).Column("[UserGroup]").Not.Nullable();
		Map(m => m.LastLogin).Not.Nullable();
		Map(m => m.Name).Length(100).Not.Nullable();
		Map(m => m.NameLC).Length(100).Not.Nullable();
		Map(m => m.NormalizedEmail).Length(50).Not.Nullable();
		Map(m => m.Password).Length(50).Not.Nullable();
		Map(m => m.PasswordHashAlgorithm).Not.Nullable();
		Map(m => m.PictureMime).Length(32).Nullable();
		Map(m => m.PreferredVideoService).Not.Nullable();
		Map(m => m.Salt).Length(100).Not.Nullable();
		Map(m => m.VerifiedArtist).Not.Nullable();

		Component(m => m.AdditionalPermissions, c =>
		{
			c.HasMany(m => m.Permissions)
				.Table("UserAdditionalPermissions")
				.AsSet()
				.KeyColumn("[User]")
				.Component(a => a.Map(m => m.Id)
					.Column("[PermissionId]"))
				.Cache.ReadWrite();
		});

		Component(m => m.Language, c =>
		{
			c.Map(m => m.CultureCode).Column("[Language]").Length(20).Not.Nullable();
		});

		/*	
			Currently the lazy loading isn't working.
			Lazy loading is not supported for non-mandatory one-to-one refereces (needs constrained, see http://stackoverflow.com/a/389345), 
			or when the referenced property is something else besides the primary key (can't use propertyref, see http://stackoverflow.com/a/3832579). 
		 
			Reference should be changed to HasMany. 
			Alternatively, the one-to-one reference could be changed to constrained, using the same primary key.
		*/
		HasOne(m => m.Options).PropertyRef(o => o.User).Cascade.All();
		//HasMany(m => m.OptionsList).Inverse().Cascade.AllDeleteOrphan();

		HasMany(m => m.AllAlbums).Inverse().Cascade.All().Cache.ReadWrite();
		HasMany(m => m.AllArtists).Inverse().Cascade.All().Cache.ReadWrite();
		HasMany(m => m.AllOwnedArtists).Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
		HasMany(m => m.AllComments).Inverse().Cascade.AllDeleteOrphan().OrderBy("Created");
		HasMany(m => m.Events).Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
		HasMany(m => m.FavoriteSongs).Inverse().Cascade.All().Cache.ReadWrite();
		HasMany(m => m.KnownLanguages).Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
		HasMany(m => m.Messages).KeyColumn("[User]").OrderBy("Created DESC").Inverse().Cascade.All();
		HasMany(m => m.OldUsernames).KeyColumn("[User]").OrderBy("[Date] DESC").Inverse().Cascade.All();
		HasMany(m => m.ReceivedMessages).KeyColumn("[Receiver]").Inverse();
		HasMany(m => m.SentMessages).KeyColumn("[Sender]").Inverse();
		HasMany(m => m.SongLists).KeyColumn("[Author]").OrderBy("Name").Inverse().Cascade.All();
		HasMany(m => m.WebLinks).Inverse().Cascade.AllDeleteOrphan().Cache.ReadWrite();
	}
}

public class UserOptionsMap : ClassMap<UserOptions>
{
	UserOptionsMap()
	{
		Table("UserOptions");
		Cache.ReadWrite();
		Id(m => m.Id);

		References(m => m.User).Column("[User]").Unique();
		Map(m => m.AboutMe).Length(int.MaxValue).Not.Nullable();
		Map(m => m.AlbumFormatString).Length(200).Not.Nullable();
		Map(m => m.CustomTitle).Length(200).Not.Nullable();
		Map(m => m.EmailVerified).Not.Nullable();
		Map(m => m.LastLoginAddress).Length(20).Not.Nullable();
		Map(m => m.Location).Length(50).Not.Nullable();
		Map(m => m.Poisoned).Not.Nullable();
		Map(m => m.PublicAlbumCollection).Not.Nullable();
		Map(m => m.PublicRatings).Not.Nullable();
		Map(m => m.Standalone).Not.Nullable();
		Map(m => m.Stylesheet).Nullable();
		Map(m => m.Supporter).Not.Nullable();
		Map(m => m.TwitterId).Not.Nullable();
		Map(m => m.TwitterName).Length(200).Not.Nullable();
		Map(m => m.TwitterOAuthToken).Length(70).Not.Nullable();
		Map(m => m.TwitterOAuthTokenSecret).Not.Nullable();
		Map(m => m.UnreadNotificationsToKeep).Not.Nullable();

		Component(m => m.LastLoginCulture, c =>
		{
			c.Map(m => m.CultureCode).Column("[LastLoginCulture]").Length(20).Not.Nullable();
		});
	}
}

public class AlbumForUserMap : ClassMap<AlbumForUser>
{
	public AlbumForUserMap()
	{
		Table("AlbumsForUsers");
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.MediaType).Not.Nullable();
		Map(m => m.PurchaseStatus).Not.Nullable();
		Map(m => m.Rating).Not.Nullable();

		References(m => m.Album).Not.Nullable().UniqueKey("IX_AlbumsForUsers");
		References(m => m.User).Not.Nullable().UniqueKey("IX_AlbumsForUsers");
	}
}

public class ArtistForUserMap : ClassMap<ArtistForUser>
{
	public ArtistForUserMap()
	{
		Table("ArtistsForUsers");
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.EmailNotifications).Not.Nullable();
		Map(m => m.SiteNotifications).Not.Nullable();

		References(m => m.Artist).Not.Nullable();
		References(m => m.User).Not.Nullable();
	}
}

public class FavoriteSongForUserMap : ClassMap<FavoriteSongForUser>
{
	public FavoriteSongForUserMap()
	{
		Table("FavoriteSongsForUsers");
		Cache.ReadWrite();
		Id(m => m.Id);

		Map(m => m.Date).Not.Nullable();
		Map(m => m.Rating).CustomType<SongVoteRating>().Not.Nullable();

		References(m => m.Song).Not.Nullable().UniqueKey("IX_FavoriteSongsForUsers_3");
		References(m => m.User).Not.Nullable().UniqueKey("IX_FavoriteSongsForUsers_3");
	}
}
