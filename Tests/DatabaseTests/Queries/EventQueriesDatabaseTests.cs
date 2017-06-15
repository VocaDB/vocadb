using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries {

	[TestClass]
	public class EventQueriesDatabaseTests {

		private readonly DatabaseTestContext<IEventRepository> context = new DatabaseTestContext<IEventRepository>();
		private readonly FakePermissionContext userContext;
		private TestDatabase Db => TestContainerManager.TestDatabase;

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

	}

}
