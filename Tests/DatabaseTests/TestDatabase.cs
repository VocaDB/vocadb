#nullable disable

using NHibernate;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.DatabaseTests;

public class TestDatabase
{
	public const int ProducerId = 257;
	public const int SongId = 121;
	public const int Song2Id = 122;
	public const int SongWithArtistId = 7787;

	public Album Album { get; private set; }
	public Album Album2 { get; private set; }
	public Album Album3 { get; private set; }

	public Artist Producer { get; private set; }
	public Artist Producer2 { get; private set; }
	public Artist Producer3 { get; private set; }

	public ReleaseEventSeries ReleaseEventSeries { get; private set; }
	public ReleaseEvent ReleaseEvent { get; private set; }
	public ReleaseEvent ReleaseEvent2 { get; private set; }

	public Song Song { get; private set; }
	public Song Song2 { get; private set; }
	public Song Song3 { get; private set; } // Song for Producer
	public Song Song4 { get; private set; }
	public Song Song5 { get; private set; }

	/// <summary>
	/// Song named "Tears"
	/// </summary>
	public Song Song6 { get; private set; }

	public Song SongWithSpecialChars { get; private set; }
	public Tag Tag { get; private set; }
	public Tag Tag2 { get; private set; }
	public Tag Tag3 { get; private set; }
	public Tag Tag4 { get; private set; }
	public User UserWithEditPermissions { get; private set; }

	public Webhook Webhook { get; private set; }
	public Webhook Webhook2 { get; private set; }
	public Webhook Webhook3 { get; private set; }
	public Webhook Webhook4 { get; private set; }

	public TestDatabase(ISessionFactory sessionFactory)
	{
		Seed(sessionFactory);
	}

	private void Seed(ISessionFactory sessionFactory)
	{
		using var session = sessionFactory.OpenSession();
		using var tx = session.BeginTransaction();
		Album = new Album(TranslatedString.Create("Re:package")) { OriginalRelease = new AlbumRelease() { ReleaseDate = new OptionalDateTime(2008) } };
		session.Save(Album);

		Album2 = new Album(TranslatedString.Create("Re:MIKUS")) { OriginalRelease = new AlbumRelease() { ReleaseDate = new OptionalDateTime(2009) } };
		session.Save(Album2);

		Album3 = new Album(TranslatedString.Create("Re:Dial"));
		session.Save(Album3);

		UserWithEditPermissions = new User("Miku", "3939", "miku@vocadb.net", PasswordHashAlgorithms.Default) { GroupId = UserGroupId.Trusted };
		UserWithEditPermissions.AdditionalPermissions.Add(PermissionToken.ViewLyrics);
		UserWithEditPermissions.AddAlbum(Album, PurchaseStatus.Nothing, MediaType.Other, 0);
		UserWithEditPermissions.AddAlbum(Album2, PurchaseStatus.Nothing, MediaType.Other, 0);
		UserWithEditPermissions.AddAlbum(Album3, PurchaseStatus.Nothing, MediaType.Other, 0);
		session.Save(UserWithEditPermissions);

		Producer = new Artist(TranslatedString.Create("Junk")) { Id = ProducerId };
		session.Save(Producer);

		Producer2 = new Artist(TranslatedString.Create("Junky"));
		session.Save(Producer2);

		Producer3 = new Artist(TranslatedString.Create("Keeno"));
		session.Save(Producer3);

		void CreateTagComment(Tag tag, string message, DateTime created, bool deleted)
		{
			var comment = new TagComment(entry: tag, message: message, loginData: new AgentLoginData(user: UserWithEditPermissions, name: UserWithEditPermissions.Name))
			{
				Created = created,
				Deleted = deleted,
			};

			tag.AllComments.Add(comment);
		}

		Tag = new Tag("electronic");
		CreateTagComment(tag: Tag, message: "1", created: new DateTime(2022, 1, 1), deleted: false);
		CreateTagComment(tag: Tag, message: "3", created: new DateTime(2022, 1, 3), deleted: true);
		CreateTagComment(tag: Tag, message: "5", created: new DateTime(2022, 1, 5), deleted: false);
		session.Save(Tag);

		Tag2 = new Tag("rock");
		Tag2.CreateName("ロック", ContentLanguageSelection.Japanese);
		CreateTagComment(tag: Tag2, message: "2", created: new DateTime(2022, 1, 2), deleted: false);
		CreateTagComment(tag: Tag2, message: "4", created: new DateTime(2022, 1, 4), deleted: true);
		CreateTagComment(tag: Tag2, message: "6", created: new DateTime(2022, 1, 6), deleted: false);
		session.Save(Tag2);

		Tag3 = new Tag("alternative rock");
		session.Save(Tag3);

		Tag4 = new Tag("techno");
		session.Save(Tag4);

		Song = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English))
		{
			Id = SongId,
			SongType = SongType.Original,
			FavoritedTimes = 1,
			PVServices = PVServices.Youtube,
			CreateDate = new DateTime(2012, 6, 1)
		};
		Song.Lyrics.Add(new LyricsForSong(Song, "Here be lyrics", string.Empty, string.Empty, TranslationType.Translation, new[] { "en" }));
		var tagUsage = new SongTagUsage(Song, Tag);
		Song.Tags.Usages.Add(tagUsage);
		Tag.AllSongTagUsages.Add(tagUsage);
		session.Save(Song);

		Song2 = new Song(new LocalizedString("Tears of Palm", ContentLanguageSelection.English))
		{
			Id = Song2Id,
			SongType = SongType.Original,
			PVServices = PVServices.Youtube,
			CreateDate = new DateTime(2012, 6, 1)
		};
		Song2.Lyrics.Add(new LyricsForSong(Song2, "Here be lyrics", string.Empty, string.Empty, TranslationType.Romanized, new[] { string.Empty }));
		session.Save(Song2);

		Song3 = new Song(new LocalizedString("Crystal Tears", ContentLanguageSelection.English))
		{
			Id = SongWithArtistId,
			FavoritedTimes = 39,
			CreateDate = new DateTime(2012, 1, 1)
		};
		Song3.AddArtist(Producer);
		session.Save(Song3);

		Song4 = new Song(new LocalizedString("Azalea", ContentLanguageSelection.English))
		{
			CreateDate = new DateTime(2012, 1, 1)
		};
		Song4.AddArtist(Producer);
		session.Save(Song4);

		Song5 = new Song(new LocalizedString("Melancholic", ContentLanguageSelection.English))
		{
			CreateDate = new DateTime(2012, 1, 1)
		};
		Song5.AddArtist(Producer2);
		session.Save(Song5);

		Song6 = new Song(new LocalizedString("Tears", ContentLanguageSelection.English))
		{
			CreateDate = new DateTime(2012, 1, 1)
		};
		Song6.AddArtist(Producer3);
		session.Save(Song6);

		SongWithSpecialChars = new Song(new LocalizedString("Nebula [Extend RMX]", ContentLanguageSelection.English))
		{
			CreateDate = new DateTime(2011, 1, 1)
		};
		session.Save(SongWithSpecialChars);

		ReleaseEvent = CreateEntry.ReleaseEvent("Miku's birthday");
		ReleaseEvent.CreateName("ミク誕生祭", ContentLanguageSelection.Japanese);
		session.Save(ReleaseEvent);
		Song.ReleaseEvent = ReleaseEvent;
		ReleaseEvent.AllSongs.Add(Song);
		session.Update(Song);

		ReleaseEventSeries = CreateEntry.EventSeries("Comiket");
		session.Save(ReleaseEventSeries);

		ReleaseEvent2 = CreateEntry.SeriesEvent(ReleaseEventSeries, 39);
		session.Save(ReleaseEvent2);

		Webhook = new Webhook("https://discord.com/api/webhooks/39", WebhookEvents.User);
		session.Save(Webhook);

		Webhook2 = new Webhook("https://discord.com/api/webhooks/3939", WebhookEvents.User | WebhookEvents.EntryReport);
		session.Save(Webhook2);

		Webhook3 = new Webhook("https://discord.com/api/webhooks/393939", WebhookEvents.EntryReport);
		session.Save(Webhook3);

		Webhook4 = new Webhook("https://discord.com/api/webhooks/39393939", WebhookEvents.EntryReport);
		session.Save(Webhook4);

		tx.Commit();
	}
}
