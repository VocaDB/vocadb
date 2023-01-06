using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Tests.DatabaseTests;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.GitHubIssues.Issue1035;

[TestClass]
public class TagQueriesDatabaseTests
{
	private readonly DatabaseTestContext<ITagRepository> _context = new();

	private static TestDatabase Db => TestContainerManager.TestDatabase;

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public void Merge_MoveComments()
	{
		var comments = _context.RunTest(repository =>
		{
			var permissionContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));

			var queries = new TagQueries(
				repository,
				permissionContext,
				new FakeEntryLinkFactory(),
				new InMemoryImagePersister(),
				new InMemoryImagePersister(),
				new FakeUserIconFactory(),
				new EnumTranslations(),
				new FakeObjectCache(),
				new FakeDiscordWebhookNotifier()
			);

			var targetId = Db.Tag2.Id;

			queries.Merge(sourceId: Db.Tag.Id, targetId: targetId);

			var target = queries.LoadTag(targetId, t => t);

			return queries.GetComments(target.Id);
		});

		comments.Length.Should().Be(4, "Number of comments");
		comments.Select(c => c.Message).Should().Equal("6", "5", "2", "1");
	}
}
