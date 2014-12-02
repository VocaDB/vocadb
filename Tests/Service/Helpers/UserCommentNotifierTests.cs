using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;

namespace VocaDb.Tests.Service.Helpers {

	/// <summary>
	/// Unit tests for <see cref="UserCommentNotifier"/>.
	/// </summary>
	[TestClass]
	public class UserCommentNotifierTests {

		private Album album;
		private string agentName;
		private EntryAnchorFactory entryLinkFactory;
		private FakeUserRepository repository;
		private User user;

		[TestInitialize]
		public void SetUp() {
			
			album = new Album(TranslatedString.Create("Synthesis")) { Id = 39 };
			agentName = "Rin";
			entryLinkFactory = new EntryAnchorFactory("http://test.vocadb.net");
			user = new User { Name = "miku" };
			repository = new FakeUserRepository(user);

		}

		private void CheckComment(string commentMsg) {

			var comment = new AlbumComment(album, commentMsg, new AgentLoginData(agentName));

			repository.HandleTransaction(ctx => {
				new UserCommentNotifier().CheckComment(comment, entryLinkFactory, ctx);				
			});

		}

		[TestMethod]
		public void CheckComment_Mentioned() {
			
			CheckComment("Hello world, @miku");

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.IsNull(notification.Sender, "Sender");
			Assert.AreEqual(user.Id, notification.Receiver.Id, "Receiver Id");
			Assert.AreEqual("Mentioned in a comment", notification.Subject, "Subject");

		}

		[TestMethod]
		public void CheckComment_NoMentions() {

			CheckComment("Hello world");

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification created");

		}

		[TestMethod]
		public void CheckComment_MentionedSkipsDisabled() {

			user.Active = false;
			CheckComment("Hello world, @miku");

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification created");

		}

	}

}
