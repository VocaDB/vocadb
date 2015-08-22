using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
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

		private PartialFindResult<UserMessageContract> CallGetList(UserInboxType inboxType, bool unread = false) {
			return queries.GetList(receiver.Id, new PagingProperties(0, 10, true), inboxType, unread, new FakeUserIconFactory());
		}

		[TestInitialize]
		public void SetUp() {
			
			sender = new User { Name = "Sender user", Id = 1};
			receiver = new User { Name = "Receiver user", Id = 2 };
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(receiver, ContentLanguagePreference.Default));

			receivedMessage = CreateEntry.UserMessageReceived(id: 1, sender: sender, receiver: receiver, subject: "Hello world", body: "Message body", read: true);
			sentMessage = CreateEntry.UserMessageSent(id: 2, sender: receiver, receiver: sender, subject: "Hello to you too", body: "Message body");
			var noPermissionMessage = CreateEntry.UserMessageReceived(id: 39, sender: sender, receiver: sender, subject: "Hello world", body: "Message body");

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
		public void GetList_All() {

			var result = CallGetList(UserInboxType.Nothing);

			Assert.AreEqual(2, result.Items.Length, "Number of messages returned");
			Assert.AreEqual(2, result.TotalCount, "Total number of messages");
			Assert.AreEqual("Hello to you too", result.Items[0].Subject, "Sent message subject");
			Assert.AreEqual("Hello world", result.Items[1].Subject, "Received message subject");

		}

		[TestMethod]
		public void GetList_Received() {

			var result = CallGetList(UserInboxType.Received).Items;

			Assert.AreEqual(1, result.Length, "Number of received messages");
			Assert.AreEqual("Hello world", result.First().Subject, "Received message subject");

		}

		[TestMethod]
		public void GetList_Sent() {

			var result = CallGetList(UserInboxType.Sent).Items;

			Assert.AreEqual(1, result.Length, "Number of sent messages");
			Assert.AreEqual("Hello to you too", result.First().Subject, "Sent message subject");

		}

		[TestMethod]
		public void GetList_Unread() {

			var anotherMsg = CreateEntry.UserMessageReceived(3, sender, receiver, "Unread message", "Unread message body", read: false);
			repository.Save(anotherMsg);

			var result = CallGetList(UserInboxType.Nothing, unread: true);

			Assert.AreEqual(1, result.Items.Length, "Number of received messages");
			Assert.AreEqual("Unread message", result.Items.First().Subject, "Received message subject");

		}

	}

}
