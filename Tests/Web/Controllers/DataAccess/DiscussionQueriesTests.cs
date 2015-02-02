using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Unit tests for <see cref="DiscussionQueries"/>.
	/// </summary>
	[TestClass]
	public class DiscussionQueriesTests {

		private DiscussionFolder folder;
		private FakeDiscussionFolderRepository repo;
		private DiscussionQueries queries;
		private User user;

		[TestInitialize]
		public void SetUp() {
			
			user = CreateEntry.User();
			folder = new DiscussionFolder("Test folder");
			repo = new FakeDiscussionFolderRepository(folder);
			repo.Save(user);
			queries = new DiscussionQueries(repo, new FakePermissionContext(user), new FakeUserIconFactory());

		}

		[TestMethod]
		public void CreateTopic() {
			
			var contract = new DiscussionTopicContract { Author = new UserWithIconContract(user), Name = "New topic", Content = "Content" };
			var result = queries.CreateTopic(folder.Id, contract);

			Assert.AreEqual(1, repo.List<DiscussionTopic>().Count, "Number of discussion topics in repo");
			var topicInRepo = repo.List<DiscussionTopic>().First();

			Assert.IsTrue(result.Id == topicInRepo.Id, "Persisted topic ID matches returned topic ID");
			Assert.AreEqual("New topic", topicInRepo.Name, "Name");
			Assert.AreEqual("Content", topicInRepo.Content, "Content");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void CreateTopic_NoPermission() {
			
			var contract = new DiscussionTopicContract { Author = new UserWithIconContract { Id = 2 }, Name = "New topic", Content = "Content" };
			queries.CreateTopic(folder.Id, contract);

		}

	}

}
