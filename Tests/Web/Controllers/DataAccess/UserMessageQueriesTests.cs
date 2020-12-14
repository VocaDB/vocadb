#nullable disable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="UserMessageQueries"/>.
	/// </summary>
	[TestClass]
	public class UserMessageQueriesTests
	{
		private UserMessageQueries queries;
		private FakePermissionContext permissionContext;
		private FakeUserMessageRepository repository;
		private User sender;
		private UserMessage receivedMessage;
		private User receiver;
		private UserMessage sentMessage;

		private UserMessageContract CallGet(int id)
		{
			return queries.Get(id, null);
		}

		private PartialFindResult<UserMessageContract> CallGetList(UserInboxType inboxType, bool unread = false)
		{
			return queries.GetList(receiver.Id, new PagingProperties(0, 10, true), inboxType, unread, null, new FakeUserIconFactory());
		}

		[TestInitialize]
		public void SetUp()
		{
			sender = new User { Name = "Sender user", Id = 1 };
			receiver = new User { Name = "Receiver user", Id = 2 };
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(receiver, ContentLanguagePreference.Default));

			var received = sender.SendMessage(receiver, "Hello world", "Message body", false);
			receivedMessage = received.Item1;
			receivedMessage.Id = 1;
			receivedMessage.Read = true;
			var sent = receiver.SendMessage(sender, "Hello to you too", "Message body", false);
			sentMessage = sent.Item1;
			sentMessage.Id = 2;
			var noPermissionMessage = CreateEntry.UserMessageReceived(id: 39, sender: sender, receiver: sender, subject: "Hello world", body: "Message body");

			repository = new FakeUserMessageRepository(sentMessage, sent.Item2, receivedMessage, received.Item2, noPermissionMessage);

			queries = new UserMessageQueries(repository, permissionContext);
		}

		[TestMethod]
		public void Get()
		{
			var result = CallGet(1);

			Assert.IsNotNull(result, "Message was loaded");
			Assert.AreEqual("Hello world", result.Subject, "Message subject");
			Assert.AreEqual("Message body", result.Body, "Message body");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void Get_NoPermission()
		{
			CallGet(39);
		}

		[TestMethod]
		public void GetList_All()
		{
			var result = CallGetList(UserInboxType.Nothing);

			Assert.AreEqual(2, result.Items.Length, "Number of messages returned");
			Assert.AreEqual(2, result.TotalCount, "Total number of messages");
			Assert.AreEqual("Hello to you too", result.Items[0].Subject, "Sent message subject");
			Assert.AreEqual("Hello world", result.Items[1].Subject, "Received message subject");
		}

		[TestMethod]
		public void GetList_Received()
		{
			var result = CallGetList(UserInboxType.Received).Items;

			Assert.AreEqual(1, result.Length, "Number of received messages");
			Assert.AreEqual("Hello world", result.First().Subject, "Received message subject");
		}

		[TestMethod]
		public void GetList_Sent()
		{
			var result = CallGetList(UserInboxType.Sent).Items;

			Assert.AreEqual(1, result.Length, "Number of sent messages");
			Assert.AreEqual("Hello to you too", result.First().Subject, "Sent message subject");
		}

		[TestMethod]
		public void GetList_Unread()
		{
			var another = sender.SendMessage(receiver, "Unread message", "Unread message body", false);
			var anotherMsg = another.Item1;
			repository.Save(anotherMsg, another.Item2);

			var result = CallGetList(UserInboxType.Nothing, unread: true);

			Assert.AreEqual(1, result.Items.Length, "Number of received messages");
			Assert.AreEqual("Unread message", result.Items.First().Subject, "Received message subject");
		}
	}
}
