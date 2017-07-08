using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries {

	[TestClass]
	public class EventQueriesDatabaseTests {

		private readonly DatabaseTestContext<IEventRepository> context = new DatabaseTestContext<IEventRepository>();
		private readonly FakePermissionContext userContext;
		private TestDatabase Db => TestContainerManager.TestDatabase;

		private ReleaseEventForEditContract Update(ReleaseEventForEditContract contract) {

			return context.RunTest(repository => {

				var queries = new EventQueries(repository, new FakeEntryLinkFactory(), userContext, new InMemoryImagePersister(), new FakeUserIconFactory(), new EnumTranslations());

				var updated = queries.Update(contract, null);

				return queries.GetEventForEdit(updated.Id);

			});

		}

		public EventQueriesDatabaseTests() {
			userContext = new FakePermissionContext(new UserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Delete() {

			context.RunTest(repository => {

				var queries = new EventQueries(repository, new FakeEntryLinkFactory(), userContext, new InMemoryImagePersister(), new FakeUserIconFactory(), new EnumTranslations());

				queries.Delete(Db.ReleaseEvent.Id, string.Empty);				

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
