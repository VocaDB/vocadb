using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service {

	[TestClass]
	public class UserServiceTests {

		private FakeUserRepository repository;
		private UserService service;

		[TestInitialize]
		public void SetUp() {
			
			repository = new FakeUserRepository();
			service = new UserService(repository, new FakePermissionContext(), new FakeEntryLinkFactory(), new FakeUserMessageMailer());

		}

		[TestMethod]
		public void CheckAccessWithKey() {

			var user = repository.Save(CreateEntry.User());

			var result = service.CheckAccessWithKey(user.Name, LoginManager.GetHashedAccessKey(user.AccessKey), "localhatsune", false);

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(user.Name, result.Name, "Name");

		}

	}

}
