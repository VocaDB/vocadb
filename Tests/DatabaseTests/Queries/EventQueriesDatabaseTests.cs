using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Tags;
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
		public void Delete() {

			context.RunTest(repository => {

				var queries = new EventQueries(repository, new FakeEntryLinkFactory(), userContext, new InMemoryImagePersister());

				queries.Delete(Db.ReleaseEvent.Id);				

			});

		}

	}

}
