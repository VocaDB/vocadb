#nullable disable

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.DatabaseTests.Queries
{
	[TestClass]
	public class EventQueriesDatabaseTests
	{
		private readonly DatabaseTestContext<IEventRepository> _context = new();
		private readonly FakeEntryLinkFactory _entryLinkFactory = new();
		private readonly EnumTranslations _enumTranslations = new();
		private readonly InMemoryImagePersister _imageStore = new();
		private readonly FakeUserMessageMailer _mailer = new();
		private readonly FakePermissionContext _userContext;
		private readonly FakeUserIconFactory _userIconFactory = new();
		private TestDatabase Db => TestContainerManager.TestDatabase;

		private Task<ReleaseEventForEditContract> Update(ReleaseEventForEditContract contract)
		{
			return _context.RunTestAsync(async repository =>
			{
				var queries = new EventQueries(repository, _entryLinkFactory, _userContext, _imageStore, _userIconFactory, _enumTranslations, _mailer,
					new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()), _imageStore);

				var updated = await queries.Update(contract, null);

				return queries.GetEventForEdit(updated.Id);
			});
		}

		public EventQueriesDatabaseTests()
		{
			_userContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(Db.UserWithEditPermissions, ContentLanguagePreference.Default));
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void MoveToTrash()
		{
			_userContext.GrantPermission(PermissionToken.MoveToTrash);

			_context.RunTest(repository =>
			{
				var id = Db.ReleaseEvent.Id;
				var queries = new EventQueries(repository, _entryLinkFactory, _userContext, _imageStore, _userIconFactory, _enumTranslations, _mailer,
					new FollowedArtistNotifier(new FakeEntryLinkFactory(), new FakeUserMessageMailer(), new EnumTranslations(), new EntrySubTypeNameFactory()), _imageStore);

				queries.MoveToTrash(id, "Deleted");

				var query = repository.HandleQuery(ctx =>
				{
					return new
					{
						EventFromDb = ctx.Get(id),
						TrashedEntry = ctx.Query<TrashedEntry>().FirstOrDefault(e => e.EntryType == EntryType.ReleaseEvent && e.EntryId == id)
					};
				});

				query.EventFromDb.Should().BeNull("Release event was deleted");
				query.TrashedEntry.Should().NotBeNull("Trashed entry was created");
				query.TrashedEntry.Notes.Should().Be("Deleted", "TrashedEntry.Notes");
			});
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public async Task Create()
		{
			var contract = new ReleaseEventForEditContract
			{
				Names = new[] {
					new LocalizedStringWithIdContract { Value = "M3 2016" }
				}
			};

			var result = await Update(contract);

			result.Should().NotBeNull("result");
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Update_DuplicateName()
		{
			// Name "Comiket 39" is already taken by ReleaseEvent2
			var contract = new ReleaseEventForEditContract(Db.ReleaseEvent, ContentLanguagePreference.Default, _userContext, null);
			contract.Names[0].Value = "Comiket 39";

			this.Awaiting(subject => subject.Update(contract)).Should().Throw<DuplicateEventNameException>();
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public void Update_DuplicateNameFromSeries()
		{
			// Generated name is "Comiket 39", which is already taken
			var contract = new ReleaseEventForEditContract(Db.ReleaseEvent, ContentLanguagePreference.Default, _userContext, null)
			{
				Series = new ReleaseEventSeriesContract(Db.ReleaseEventSeries, ContentLanguagePreference.English),
				SeriesNumber = 39
			};

			this.Awaiting(subject => subject.Update(contract)).Should().Throw<DuplicateEventNameException>();
		}

		[TestMethod]
		[TestCategory(TestCategories.Database)]
		public async Task Update_SwapNameTranslations()
		{
			var contract = new ReleaseEventForEditContract(Db.ReleaseEvent, ContentLanguagePreference.Default, _userContext, null);
			contract.Names[0].Value = "ミク誕生祭"; // Swap values
			contract.Names[1].Value = "Miku's birthday";

			var result = await Update(contract);

			result.Names.Length.Should().Be(2, "Number of names");
			var name = result.Names[0];
			name.Value.Should().Be("ミク誕生祭", "Name value");
		}
	}
}
