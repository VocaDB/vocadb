using System;
using NHibernate;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.DatabaseTests {

	public class TestDatabase {

		public const int ProducerId = 257;
		public const int SongId = 121;
		public const int Song2Id = 122;
		public const int SongWithArtistId = 7787;

		public Artist Producer { get; private set; }
		public Artist Producer2 { get; private set; }
		public Artist Producer3 { get; private set; }
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

		public TestDatabase(ISessionFactory sessionFactory) {
			Seed(sessionFactory);
		}

		private void Seed(ISessionFactory sessionFactory) {
			
			using (var session = sessionFactory.OpenSession())
			using (var tx = session.BeginTransaction()) {
				
				Producer = new Artist(TranslatedString.Create("Junk")) { Id = ProducerId };
				session.Save(Producer);

				Producer2 = new Artist(TranslatedString.Create("Junky"));
				session.Save(Producer2);

				Producer3 = new Artist(TranslatedString.Create("Keeno"));
				session.Save(Producer3);

				Tag = new Tag("electronic");
				session.Save(Tag);

				Tag2 = new Tag("rock");
				Tag2.CreateName("ロック", ContentLanguageSelection.Japanese);
				session.Save(Tag2);

				Tag3 = new Tag("alternative rock");
				session.Save(Tag3);

				Tag4 = new Tag("techno");
				session.Save(Tag4);

				Song = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English)) {
					Id = SongId, SongType = SongType.Original, FavoritedTimes = 1, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1)
				};
				Song.Lyrics.Add(new LyricsForSong(Song, ContentLanguageSelection.English, "Here be lyrics", string.Empty));
				var tagUsage = new SongTagUsage(Song, Tag);
				Song.Tags.Usages.Add(tagUsage);
				Tag.AllSongTagUsages.Add(tagUsage);
				session.Save(Song);

				Song2 = new Song(new LocalizedString("Tears of Palm", ContentLanguageSelection.English)) {
					Id = Song2Id, SongType = SongType.Original, PVServices = PVServices.Youtube, CreateDate = new DateTime(2012, 6, 1)
				};
				Song2.Lyrics.Add(new LyricsForSong(Song2, ContentLanguageSelection.Romaji, "Here be lyrics", string.Empty));
				session.Save(Song2);

				Song3 = new Song(new LocalizedString("Crystal Tears", ContentLanguageSelection.English)) {
					Id = SongWithArtistId, FavoritedTimes = 39, CreateDate = new DateTime(2012, 1, 1)
				};
				Song3.AddArtist(Producer);
				session.Save(Song3);

				Song4 = new Song(new LocalizedString("Azalea", ContentLanguageSelection.English)) {
					CreateDate = new DateTime(2012, 1, 1)
				};
				Song4.AddArtist(Producer);
				session.Save(Song4);

				Song5 = new Song(new LocalizedString("Melancholic", ContentLanguageSelection.English)) {
					CreateDate = new DateTime(2012, 1, 1)
				};
				Song5.AddArtist(Producer2);
				session.Save(Song5);

				Song6 = new Song(new LocalizedString("Tears", ContentLanguageSelection.English)) {
					CreateDate = new DateTime(2012, 1, 1)
				};
				Song6.AddArtist(Producer3);
				session.Save(Song6);

				SongWithSpecialChars = new Song(new LocalizedString("Nebula [Extend RMX]", ContentLanguageSelection.English)) {
					CreateDate = new DateTime(2011, 1, 1)
				};
				session.Save(SongWithSpecialChars);

				UserWithEditPermissions = new User("Miku", "3939", "miku@vocadb.net", 3939) { GroupId = UserGroupId.Trusted };
				session.Save(UserWithEditPermissions);

				tx.Commit();

			}

		}

	}
}
