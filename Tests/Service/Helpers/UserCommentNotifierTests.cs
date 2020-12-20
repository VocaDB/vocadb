#nullable disable

using System.Linq;
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

			Assert.IsNotNull(notification, "Notification was created");
			Assert.IsNull(notification.Sender, "Sender");
			Assert.AreEqual(_user.Id, notification.Receiver.Id, "Receiver Id");
			Assert.AreEqual("Mentioned in a comment", notification.Subject, "Subject");
		}

		[TestMethod]
		public void CheckComment_Mentioned_Multiple()
		{
			CheckComment("Hello world, @miku @luka");

			var messages = _repository.List<UserMessage>();
			Assert.AreEqual(2, messages.Count, "Messages generated for both users");
			Assert.AreEqual(_user, messages[0].Receiver, "First user as expected");
		}

		[TestMethod]
		public void CheckComment_NoMentions()
		{
			CheckComment("Hello world");

			Assert.IsFalse(_repository.List<UserMessage>().Any(), "No notification created");
		}

		[TestMethod]
		public void CheckComment_MentionedSkipsDisabled()
		{
			_user.Active = false;
			CheckComment("Hello world, @miku");

			Assert.IsFalse(_repository.List<UserMessage>().Any(), "No notification created");
		}
	}
}
