#nullable disable

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;

namespace VocaDb.Tests.Service.Helpers
{
	/// <summary>
	/// Unit tests for <see cref="UserCommentNotifier"/>.
	/// </summary>
	[TestClass]
	public class UserCommentNotifierTests
	{
		private Album _album;
		private string _agentName;
		private EntryAnchorFactory _entryLinkFactory;
		private FakeUserRepository _repository;
		private User _user;
		private User _user2;

		[TestInitialize]
		public void SetUp()
		{
			_album = new Album(TranslatedString.Create("Synthesis")) { Id = 39 };
			_agentName = "Rin";
			_entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");
			_user = CreateEntry.User(name: "miku");
			_user2 = CreateEntry.User(name: "luka");
			_repository = new FakeUserRepository(_user, _user2);
		}

		private void CheckComment(string commentMsg)
		{
			var comment = new AlbumComment(_album, commentMsg, new AgentLoginData(_agentName));

			_repository.HandleTransaction(ctx =>
			{
				new UserCommentNotifier().CheckComment(comment, _entryLinkFactory, ctx);
			});
		}

		[TestMethod]
		public void CheckComment_Mentioned()
		{
			CheckComment("Hello world, @miku");

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			notification.Should().NotBeNull("Notification was created");
			notification.Sender.Should().BeNull("Sender");
			notification.Receiver.Id.Should().Be(_user.Id, "Receiver Id");
			notification.Subject.Should().Be("Mentioned in a comment", "Subject");
		}

		[TestMethod]
		public void CheckComment_Mentioned_Multiple()
		{
			CheckComment("Hello world, @miku @luka");

			var messages = _repository.List<UserMessage>();
			messages.Count.Should().Be(2, "Messages generated for both users");
			messages[0].Receiver.Should().Be(_user, "First user as expected");
		}

		[TestMethod]
		public void CheckComment_NoMentions()
		{
			CheckComment("Hello world");

			_repository.List<UserMessage>().Any().Should().BeFalse("No notification created");
		}

		[TestMethod]
		public void CheckComment_MentionedSkipsDisabled()
		{
			_user.Active = false;
			CheckComment("Hello world, @miku");

			_repository.List<UserMessage>().Any().Should().BeFalse("No notification created");
		}
	}
}
