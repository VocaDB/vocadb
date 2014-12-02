using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Controllers.DataAccess;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="UserMessageQueries"/>.
	/// </summary>
	[TestClass]
	public class UserMessageQueriesTests {

		private UserMessageQueries queries;
		private FakePermissionContext permissionContext;
		private FakeUserMessageRepository repository;
		private User sender;
		private UserMessage receivedMessage;
		private User receiver;
		private UserMessage sentMessage;

		private UserMessageContract CallGet(int id) {
			return queries.Get(id, null);
		}

		private UserMessagesContract CallGetList(bool unread = false) {
			return queries.GetList(receiver.Id, new PagingProperties(0, 10, false), unread, new FakeUserIconFactory());
		}

		[TestInitialize]
		public void SetUp() {
			
			sender = new User { Name = "Sender user", Id = 1};
			receiver = new User { Name = "Receiver user", Id = 2 };
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(receiver, ContentLanguagePreference.Default));

			receivedMessage = CreateEntry.UserMessage(id: 1, sender: sender, receiver: receiver, subject: "Hello world", body: "Message body", read: true);
			sentMessage = CreateEntry.UserMessage(id: 2, sender: receiver, receiver: sender, subject: "Hello to you too", body: "Message body");
			var noPermissionMessage = CreateEntry.UserMessage(id: 39, sender: sender, receiver: sender, subject: "Hello world", body: "Message body");

			repository = new FakeUserMessageRepository(sentMessage, receivedMessage, noPermissionMessage);

			queries = new UserMessageQueries(repository, permissionContext);

		}

		[TestMethod]
		public void Get() {

			var result = CallGet(1);

			Assert.IsNotNull(result, "Message was loaded");
			Assert.AreEqual("Hello world", result.Subject, "Message subject");
			Assert.AreEqual("Message body", result.Body, "Message body");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Get_NoPermission() {


			CallGet(39);

		}

		[TestMethod]
		public void GetList() { 

			var result = CallGetList();

			Assert.AreEqual(1, result.ReceivedMessages.Length, "Number of received messages");
			Assert.AreEqual("Hello world", result.ReceivedMessages.First().Subject, "Received message subject");
			Assert.AreEqual(1, result.SentMessages.Length, "Number of sent messages");
			Assert.AreEqual("Hello to you too", result.SentMessages.First().Subject, "Sent message subject");

		}

		[TestMethod]
		public void GetList_Unread() {
			
			var anotherMsg = CreateEntry.UserMessage(3, sender, receiver, "Unread message", "Unread message body", read: false);
			repository.Save(anotherMsg);

			var result = CallGetList(unread: true);

			Assert.AreEqual(1, result.ReceivedMessages.Length, "Number of received messages");
			Assert.AreEqual("Unread message", result.ReceivedMessages.First().Subject, "Received message subject");
			Assert.AreEqual(0, result.SentMessages.Length, "Number of sent messages");

		}

	}

}
