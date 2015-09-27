using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service {

	[TestClass]
	public class UserServiceTests {

		private FakeUserRepository repository;
		private UserService service;
		private User user;

		[TestInitialize]
		public void SetUp() {

			user = CreateEntry.User();
			repository = new FakeUserRepository(user);
			service = new UserService(repository, new FakePermissionContext(), new FakeEntryLinkFactory(), new FakeUserMessageMailer());

		}

		[TestMethod]
		public void CheckAccessWithKey_Valid() {

			var result = service.CheckAccessWithKey(user.Name, LoginManager.GetHashedAccessKey(user.AccessKey), "localhatsune", false);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(user.Name, result.Name, "Name");

		}

		[TestMethod]
		public void CheckAccessWithKey_Invalid() {

			var result = service.CheckAccessWithKey(user.Name, LoginManager.GetHashedAccessKey("rinrin"), "localhatsune", false);

			Assert.IsNull(result, "result");
			
		}

	}

}
