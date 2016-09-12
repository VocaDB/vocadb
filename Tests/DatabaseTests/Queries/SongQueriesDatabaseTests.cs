using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils.Config;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries {

	/// <summary>
	/// Database tests for <see cref="SongQueries"/>.
	/// </summary>
	[TestClass]
	public class SongQueriesDatabaseTests {

		private readonly DatabaseTestContext<ISongRepository> context = new DatabaseTestContext<ISongRepository>();
		private readonly FakePermissionContext userContext;
		private TestDatabase Db => TestContainerManager.TestDatabase;

		public SongQueriesDatabaseTests() {
			userContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));
		}

		[TestMethod]
		public void Update_Lyrics() {

			var song = context.RunTest(repository => {

				var queries = new SongQueries(repository, userContext, new FakeEntryLinkFactory(), new FakePVParser(),
					new FakeUserMessageMailer(), new FakeLanguageDetector(), new FakeUserIconFactory(), new EnumTranslations(), new FakeObjectCache(), new VdbConfigManager());

				var contract = new SongForEditContract(Db.Song2, ContentLanguagePreference.English);

				contract.Lyrics = new[] {
					new LyricsForSongContract { TranslationType = TranslationType.Original, CultureCode = "ja-JP", Source = string.Empty, URL = string.Empty, Value = "Miku!" }
				};

				var updated = queries.UpdateBasicProperties(contract);

				return queries.GetSongForEdit(updated.Id);

			});

			Assert.AreEqual(1, song.Lyrics.Length, "Lyrics created");

		}

	}

}
