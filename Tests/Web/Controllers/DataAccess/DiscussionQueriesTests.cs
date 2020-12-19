#nullable disable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Discussions;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Unit tests for <see cref="DiscussionQueries"/>.
	/// </summary>
	[TestClass]
	public class DiscussionQueriesTests
	{
		private DiscussionFolder _folder;
		private FakeDiscussionFolderRepository _repo;
		private DiscussionQueries _queries;
		private User _user;

		[TestInitialize]
		public void SetUp()
		{
			_user = CreateEntry.User();
			_folder = new DiscussionFolder("Test folder");
			_repo = new FakeDiscussionFolderRepository(_folder);
			_repo.Save(_user);
			_queries = new DiscussionQueries(_repo, new FakePermissionContext(_user), new FakeUserIconFactory(), new FakeEntryLinkFactory());
		}

		[TestMethod]
		public void CreateTopic()
		{
			var contract = new DiscussionTopicContract { Author = new UserForApiContract(_user), Name = "New topic", Content = "Content" };
			var result = _queries.CreateTopic(_folder.Id, contract);

			Assert.AreNotEqual(0, result.Id, "Id was assigned");
			Assert.AreEqual(1, _repo.List<DiscussionTopic>().Count, "Number of discussion topics in repo");

			Assert.AreEqual(1, _folder.AllTopics.Count, "Topic was added to folder");
			Assert.IsTrue(result.Id == _folder.AllTopics.First().Id, "Topic Id in folder matches returned topic ID");

			var topicInRepo = _repo.List<DiscussionTopic>().First();

			Assert.IsTrue(result.Id == topicInRepo.Id, "Persisted topic ID matches returned topic ID");
			Assert.AreEqual("New topic", topicInRepo.Name, "Name");
			Assert.AreEqual("Content", topicInRepo.Content, "Content");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void CreateTopic_NoPermission()
		{
			var contract = new DiscussionTopicContract { Author = new UserForApiContract { Id = 2 }, Name = "New topic", Content = "Content" };
			_queries.CreateTopic(_folder.Id, contract);
		}
	}
}
