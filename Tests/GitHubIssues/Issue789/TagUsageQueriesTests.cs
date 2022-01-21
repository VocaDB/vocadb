using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Queries;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.GitHubIssues.Issue789
{
	[TestClass]
	public class TagUsageQueriesTests
	{
		private Song _entry = default!;
		private Tag _existingTag = default!;
		private readonly FakeUserRepository _repository = new();
		private TagUsageQueries _queries = default!;
		private User _user = default!;

		private Task AddSongTags(int entryId, params TagBaseContract[] tags)
		{
			return _queries.AddTags<Song, SongTagUsage>(
				entryId: entryId,
				tags: tags,
				onlyAdd: false,
				repository: _repository,
				entryLinkFactory: new FakeEntryLinkFactory(),
				enumTranslations: new EnumTranslations(),
				tagFunc: song => song.Tags,
				tagUsageFactoryFactory: (song, ctx) => new SongTagUsageFactory(ctx, song)
			);
		}

		private Task AddSongListTags(int entryId, params TagBaseContract[] tags)
		{
			return _queries.AddTags<SongList, SongListTagUsage>(
				entryId: entryId,
				tags: tags,
				onlyAdd: false,
				repository: _repository,
				entryLinkFactory: new FakeEntryLinkFactory(),
				enumTranslations: new EnumTranslations(),
				tagFunc: songList => songList.Tags,
				tagUsageFactoryFactory: (songList, ctx) => new SongListTagUsageFactory(ctx, songList)
			);
		}

		private TagBaseContract Contract(int id)
		{
			return new TagBaseContract { Id = id };
		}

		private TagBaseContract Contract(string name)
		{
			return new TagBaseContract { Name = name };
		}

		[TestInitialize]
		public void SetUp()
		{
			_user = _repository.Save(CreateEntry.User(group: UserGroupId.Regular));
			_queries = new TagUsageQueries(new FakePermissionContext(_user));
			var song = CreateEntry.Song(name: "Puppet");
			song.Status = EntryStatus.Locked;
			_entry = _repository.Save(song);
			_existingTag = _repository.Save(CreateEntry.Tag("techno"));
		}

		[TestMethod]
		public void AddNewTagByName()
		{
			var tags = new[] { Contract("vocarock") };

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, tags)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void AddExistingTagByName()
		{
			_repository.Save(CreateEntry.Tag("vocarock", 39));

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, Contract("vocarock"))).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void AddTagById()
		{
			var tag = _repository.Save(CreateEntry.Tag("vocarock"));

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, Contract(tag.Id))).Should().Throw<NotAllowedException>();
		}

		/// <summary>
		/// Add tag based on translated name
		/// </summary>
		[TestMethod]
		public void AddTagByTranslation()
		{
			var tag = _repository.Save(CreateEntry.Tag("rock"));
			tag.CreateName("ロック", ContentLanguageSelection.Japanese);

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, Contract("ロック"))).Should().Throw<NotAllowedException>();
		}

		/// <summary>
		/// Add renamed tag by name
		/// </summary>
		[TestMethod]
		public void AddNewTag_TagIsRenamed()
		{
			var tag = _repository.Save(CreateEntry.Tag("vocarock", 39));
			tag.Names.First().Value = "rock";
			tag.Names.UpdateSortNames();

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, Contract("vocarock"))).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void SkipDuplicates()
		{
			var tag = _repository.Save(CreateEntry.Tag("rock"));
			tag.CreateName("ロック", ContentLanguageSelection.Japanese);

			var tags = new[] {
				Contract(tag.Id),
				Contract(tag.DefaultName),
				Contract("ロック")
			};

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, tags)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void AddMultiple()
		{
			var tag1 = _repository.Save(CreateEntry.Tag("vocarock"));

			var tags = new[] {
				new TagBaseContract { Id = tag1.Id },
				new TagBaseContract { Name = "power metal" }
			};

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, tags)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void AddAndRemoveMultiple()
		{
			var tag1 = _repository.Save(CreateEntry.Tag("vocarock"));

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, new TagBaseContract { Id = tag1.Id })).Should().Throw<NotAllowedException>();

			var tags = new[] {
				new TagBaseContract { Name = "power metal" }
			};

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, tags)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void RemoveTagUsage()
		{
			var tag = _repository.Save(CreateEntry.Tag("rock"));
			var tag2 = _repository.Save(CreateEntry.Tag("metal"));
			var usage = _repository.Save(_entry.AddTag(tag).Result);
			_repository.Save(_entry.AddTag(tag2).Result);

			_queries.Invoking(subject => subject.RemoveTagUsage<SongTagUsage, Song>(usage.Id, _repository.OfType<Song>())).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void AddTag_SendNotifications()
		{
			var followingUser = _repository.Save(CreateEntry.User(name: "Rin"));
			var tag = _repository.Save(CreateEntry.Tag("rock"));
			_repository.Save(followingUser.AddTag(tag));

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, Contract(tag.Id))).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void AddTag_SendNotifications_IgnoreSelf()
		{
			var tag = _repository.Save(CreateEntry.Tag("rock"));
			_repository.Save(_user.AddTag(tag));

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, Contract(tag.Id))).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public async Task AddTag_SendNotifications_IgnorePersonalSongList()
		{
			var followingUser = _repository.Save(CreateEntry.User(name: "Rin"));
			var tag = _repository.Save(CreateEntry.Tag("rock"));
			_repository.Save(followingUser.AddTag(tag));

			var list = _repository.Save(new SongList("Mikulist", _user));
			await AddSongListTags(list.Id, Contract(tag.Id));

			_repository.List<UserMessage>().Count.Should().Be(0, "No message was sent");
		}

		[TestMethod]
		public void SkipInvalidTarget()
		{
			_existingTag.Targets = TagTargetTypes.Album;
			var tag = _repository.Save(CreateEntry.Tag("vocarock", 39));

			this.Awaiting(subject => subject.AddSongTags(_entry.Id, Contract(_existingTag.Id), Contract(tag.Id))).Should().Throw<NotAllowedException>();
		}
	}
}
