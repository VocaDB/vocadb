#nullable disable

using System.Linq;
using FluentAssertions;
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
		private UserMessageQueries _queries;
		private FakePermissionContext _permissionContext;
		private FakeUserMessageRepository _repository;
		private User _sender;
		private UserMessage _receivedMessage;
		private User _receiver;
		private UserMessage _sentMessage;

		private UserMessageContract CallGet(int id)
		{
			return _queries.Get(id, null);
		}

		private PartialFindResult<UserMessageContract> CallGetList(UserInboxType inboxType, bool unread = false)
		{
			return _queries.GetList(_receiver.Id, new PagingProperties(0, 10, true), inboxType, unread, null, new FakeUserIconFactory());
		}

		[TestInitialize]
		public void SetUp()
		{
			_sender = new User { Name = "Sender user", Id = 1 };
			_receiver = new User { Name = "Receiver user", Id = 2 };
			_permissionContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(_receiver, ContentLanguagePreference.Default));

			var received = _sender.SendMessage(_receiver, "Hello world", "Message body", false);
			_receivedMessage = received.Item1;
			_receivedMessage.Id = 1;
			_receivedMessage.Read = true;
			var sent = _receiver.SendMessage(_sender, "Hello to you too", "Message body", false);
			_sentMessage = sent.Item1;
			_sentMessage.Id = 2;
			var noPermissionMessage = CreateEntry.UserMessageReceived(id: 39, sender: _sender, receiver: _sender, subject: "Hello world", body: "Message body");

			_repository = new FakeUserMessageRepository(_sentMessage, sent.Item2, _receivedMessage, received.Item2, noPermissionMessage);

			_queries = new UserMessageQueries(_repository, _permissionContext);
		}

		[TestMethod]
		public void Get()
		{
			var result = CallGet(1);

			result.Should().NotBeNull("Message was loaded");
			result.Subject.Should().Be("Hello world", "Message subject");
			result.Body.Should().Be("Message body", "Message body");
		}

		[TestMethod]
		public void Get_NoPermission()
		{
			this.Invoking(subject => subject.CallGet(39)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void GetList_All()
		{
			var result = CallGetList(UserInboxType.Nothing);

			result.Items.Length.Should().Be(2, "Number of messages returned");
			result.TotalCount.Should().Be(2, "Total number of messages");
			result.Items[0].Subject.Should().Be("Hello to you too", "Sent message subject");
			result.Items[1].Subject.Should().Be("Hello world", "Received message subject");
		}

		[TestMethod]
		public void GetList_Received()
		{
			var result = CallGetList(UserInboxType.Received).Items;

			result.Length.Should().Be(1, "Number of received messages");
			result.First().Subject.Should().Be("Hello world", "Received message subject");
		}

		[TestMethod]
		public void GetList_Sent()
		{
			var result = CallGetList(UserInboxType.Sent).Items;

			result.Length.Should().Be(1, "Number of sent messages");
			result.First().Subject.Should().Be("Hello to you too", "Sent message subject");
		}

		[TestMethod]
		public void GetList_Unread()
		{
			var another = _sender.SendMessage(_receiver, "Unread message", "Unread message body", false);
			var anotherMsg = another.Item1;
			_repository.Save(anotherMsg, another.Item2);

			var result = CallGetList(UserInboxType.Nothing, unread: true);

			result.Items.Length.Should().Be(1, "Number of received messages");
			result.Items.First().Subject.Should().Be("Unread message", "Received message subject");
		}
	}
}
