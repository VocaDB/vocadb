using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.GitHubIssues.Issue1035
{
	[TestClass]
	public class TagQueriesTests
	{
		private InMemoryImagePersister _imagePersister = default!;
		private FakePermissionContext _permissionContext = default!;
		private TagQueries _queries = default!;
		private FakeTagRepository _repository = default!;
		private Tag _tag = default!;
		private User _user = default!;

		private Tag CreateAndSaveTag(string englishName)
		{
			var t = CreateEntry.Tag(englishName);

			_repository.Save(t);

			foreach (var name in t.Names)
				_repository.Save(name);

			return t;
		}

		[TestInitialize]
		public void SetUp()
		{
			_repository = new FakeTagRepository();

			_tag = CreateAndSaveTag("Appearance Miku");

			_user = new User("User", "123", "test@test.com", PasswordHashAlgorithms.Default) { GroupId = UserGroupId.Moderator };
			_repository.Add(_user);

			_permissionContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(_user, ContentLanguagePreference.Default));

			_imagePersister = new InMemoryImagePersister();
			_queries = new TagQueries(
				_repository,
				_permissionContext,
				new FakeEntryLinkFactory(),
				_imagePersister,
				_imagePersister,
				new FakeUserIconFactory(),
				new EnumTranslations(),
				new FakeObjectCache(),
				new FakeDiscordWebhookNotifier()
			);
		}

		[TestMethod]
		public void Merge_Comments()
		{
			void CreateTagComment(Tag tag, string message, DateTime created, bool deleted)
			{
				var comment = new TagComment(entry: tag, message: message, loginData: new AgentLoginData(user: _user, name: _user.Name))
				{
					Created = created,
					Deleted = deleted,
				};

				tag.AllComments.Add(comment);
			}

			CreateTagComment(tag: _tag, message: "1", created: new DateTime(2022, 1, 1), deleted: false);
			CreateTagComment(tag: _tag, message: "3", created: new DateTime(2022, 1, 3), deleted: true);
			CreateTagComment(tag: _tag, message: "5", created: new DateTime(2022, 1, 5), deleted: false);

			var target = _repository.Save(new Tag("target"));

			CreateTagComment(tag: target, message: "2", created: new DateTime(2022, 1, 2), deleted: false);
			CreateTagComment(tag: target, message: "4", created: new DateTime(2022, 1, 4), deleted: true);
			CreateTagComment(tag: target, message: "6", created: new DateTime(2022, 1, 6), deleted: false);

			_queries.Merge(sourceId: _tag.Id, targetId: target.Id);

			target.Comments.Count().Should().Be(4, "Number of comments");
			target.Comments.OrderByDescending(c => c.Created).Select(c => c.Message).Should().Equal("6", "5", "2", "1");
		}
	}
}
