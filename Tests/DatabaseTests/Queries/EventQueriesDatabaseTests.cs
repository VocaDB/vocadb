using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries {

	[TestClass]
	public class EventQueriesDatabaseTests {

		private readonly DatabaseTestContext<IEventRepository> context = new DatabaseTestContext<IEventRepository>();
		private readonly FakeEntryLinkFactory entryLinkFactory = new FakeEntryLinkFactory();
		private readonly EnumTranslations enumTranslations = new EnumTranslations();
		private readonly InMemoryImagePersister imageStore = new InMemoryImagePersister();
		private readonly FakeUserMessageMailer mailer = new FakeUserMessageMailer();
		private readonly FakePermissionContext userContext;
		private readonly FakeUserIconFactory userIconFactory = new FakeUserIconFactory();
		private TestDatabase Db => TestContainerManager.TestDatabase;

		private ReleaseEventForEditContract Update(ReleaseEventForEditContract contract) {

			return context.RunTest(repository => {

				var queries = new EventQueries(repository, entryLinkFactory, userContext, imageStore, userIconFactory, enumTranslations, mailer, 
					new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()));

				var updated = queries.Update(contract, null);

				return queries.GetEventForEdit(updated.Id);

			});

		}

		public EventQueriesDatabaseTests() {
			userContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void MoveToTrash() {

			userContext.GrantPermission(PermissionToken.MoveToTrash);

			context.RunTest(repository => {

				var id = Db.ReleaseEvent.Id;
				var queries = new EventQueries(repository, entryLinkFactory, userContext, imageStore, userIconFactory, enumTranslations, mailer, 
					new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()));

				queries.MoveToTrash(id, "Deleted");

				var query = repository.HandleQuery(ctx => {
					return new {
						EventFromDb = ctx.Get(id),
						TrashedEntry = ctx.Query<TrashedEntry>().FirstOrDefault(e => e.EntryType == EntryType.ReleaseEvent && e.EntryId == id)
					};
				});

				Assert.IsNull(query.EventFromDb, "Release event was deleted");
				Assert.IsNotNull(query.TrashedEntry, "Trashed entry was created");
				Assert.AreEqual("Deleted", query.TrashedEntry.Notes, "TrashedEntry.Notes");

			});

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Create() {
			
			var contract = new ReleaseEventForEditContract {
				Names = new[] {
					new LocalizedStringWithIdContract { Value = "M3 2016" }
				}
			};

			var result = Update(contract);

			Assert.IsNotNull(result, "result");

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void Update_DuplicateName() {

			// Name "Comiket 39" is already taken by ReleaseEvent2
			var contract = new ReleaseEventForEditContract(Db.ReleaseEvent, ContentLanguagePreference.Default, userContext, null);
			contract.Names[0].Value = "Comiket 39";

			Update(contract);

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		[ExpectedException(typeof(DuplicateEventNameException))]
		public void Update_DuplicateNameFromSeries() {

			// Generated name is "Comiket 39", which is already taken
			var contract = new ReleaseEventForEditContract(Db.ReleaseEvent, ContentLanguagePreference.Default, userContext, null) {
				Series = new ReleaseEventSeriesContract(Db.ReleaseEventSeries, ContentLanguagePreference.English),
				SeriesNumber = 39
			};

			Update(contract);

		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Update_SwapNameTranslations() {

			var contract = new ReleaseEventForEditContract(Db.ReleaseEvent, ContentLanguagePreference.Default, userContext, null);
			contract.Names[0].Value = "ミク誕生祭"; // Swap values
			contract.Names[1].Value = "Miku's birthday";

			var result = Update(contract);

			Assert.AreEqual(2, result.Names.Length, "Number of names");
			var name = result.Names[0];
			Assert.AreEqual("ミク誕生祭", name.Value, "Name value");

		}

	}

}
