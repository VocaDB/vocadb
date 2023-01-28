#nullable disable

using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Queries;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Service.Queries;

/// <summary>
/// Tests for <see cref="TagUsageQueries"/>.
/// </summary>
[TestClass]
public class TagUsageQueriesTests
{
	private Song _entry;
	private Tag _existingTag;
	private readonly FakeUserRepository _repository = new();
	private TagUsageQueries _queries;
	private User _user;

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
		_user = _repository.Save(CreateEntry.User(group: UserGroupId.Trusted));
		_queries = new TagUsageQueries(new FakePermissionContext(_user));
		_entry = _repository.Save(CreateEntry.Song(name: "Puppet"));
		_existingTag = _repository.Save(CreateEntry.Tag("techno"));
	}

	[TestMethod]
	public async Task AddNewTagByName()
	{
		var tags = new[] { Contract("vocarock") };

		await AddSongTags(_entry.Id, tags);

		_entry.Tags.Tags.Count().Should().Be(1, "Number of tags");
		var usage = _entry.Tags.Usages.First();
		usage.Tag.DefaultName.Should().Be("vocarock", "Added tag name");
		usage.HasVoteByUser(_user).Should().BeTrue("Vote is by the logged in user");
		usage.Tag.UsageCount.Should().Be(1, "Number of usages for tag");

		_repository.List<Tag>().Count.Should().Be(2, "Number of tags in the repository");
		_repository.List<Tag>().Contains(usage.Tag).Should().BeTrue("Tag in repository is the same as the one applied");
	}

	[TestMethod]
	public async Task AddExistingTagByName()
	{
		_repository.Save(CreateEntry.Tag("vocarock", 39));

		await AddSongTags(_entry.Id, Contract("vocarock"));

		var entryTags = _entry.Tags.Tags.ToArray();
		entryTags.Length.Should().Be(1, "Number of tags");
		entryTags.Any(t => t.Id == 39).Should().BeTrue("vocarock tag is added");
	}

	[TestMethod]
	public async Task AddTagById()
	{
		var tag = _repository.Save(CreateEntry.Tag("vocarock"));

		await AddSongTags(_entry.Id, Contract(tag.Id));

		_entry.Tags.Tags.Count().Should().Be(1, "Number of tags");
		var usage = _entry.Tags.Usages.First();
		usage.Tag.DefaultName.Should().Be("vocarock", "Added tag name");
		usage.Tag.Id.Should().Be(tag.Id, "Added tag Id");
		tag.UsageCount.Should().Be(1, "Number of usages for tag");
	}

	/// <summary>
	/// Add tag based on translated name
	/// </summary>
	[TestMethod]
	public async Task AddTagByTranslation()
	{
		var tag = _repository.Save(CreateEntry.Tag("rock"));
		tag.CreateName("ロック", ContentLanguageSelection.Japanese);

		await AddSongTags(_entry.Id, Contract("ロック"));

		_entry.Tags.Tags.Count().Should().Be(1, "Number of tags");
		var usage = _entry.Tags.Usages.First();
		usage.Tag.DefaultName.Should().Be("rock", "Added tag name");
		usage.Tag.Id.Should().Be(tag.Id, "Added tag Id");
	}

	/// <summary>
	/// Add renamed tag by name
	/// </summary>
	[TestMethod]
	public async Task AddNewTag_TagIsRenamed()
	{
		var tag = _repository.Save(CreateEntry.Tag("vocarock", 39));
		tag.Names.First().Value = "rock";
		tag.Names.UpdateSortNames();

		// Attempting to add tag "vocarock". The "vocarock" tag was renamed as "rock" so this is a new tag.
		await AddSongTags(_entry.Id, Contract("vocarock"));

		var entryTags = _entry.Tags.Tags.ToArray();
		entryTags.Length.Should().Be(1, "Number of tags");
		entryTags.Any(t => t.DefaultName == "vocarock").Should().BeTrue("vocarock tag is added");
		entryTags.Any(t => t.Id != 39).Should().BeTrue("tag name cannot be the same as rock tag");
	}

	[TestMethod]
	public async Task SkipDuplicates()
	{
		var tag = _repository.Save(CreateEntry.Tag("rock"));
		tag.CreateName("ロック", ContentLanguageSelection.Japanese);

		var tags = new[] {
			Contract(tag.Id),
			Contract(tag.DefaultName),
			Contract("ロック")
		};

		await AddSongTags(_entry.Id, tags);

		_entry.Tags.Tags.Count().Should().Be(1, "Number of tags");
		var usage = _entry.Tags.Usages.First();
		usage.Tag.Id.Should().Be(tag.Id, "Added tag name");
		tag.UsageCount.Should().Be(1, "Number of usages for tag");
	}

	[TestMethod]
	public async Task AddMultiple()
	{
		var tag1 = _repository.Save(CreateEntry.Tag("vocarock"));

		var tags = new[] {
			new TagBaseContract { Id = tag1.Id },
			new TagBaseContract { Name = "power metal" }
		};

		await AddSongTags(_entry.Id, tags);

		var entryTags = _entry.Tags.Tags.ToArray();
		entryTags.Length.Should().Be(2, "Number of applied tags");
		entryTags.Any(t => t.DefaultName == "vocarock").Should().BeTrue("vocarock tag is added");
		entryTags.Any(t => t.DefaultName == "power metal").Should().BeTrue("power metal tag is added");
		entryTags[0].UsageCount.Should().Be(1, "Number of usages for tag");
	}

	[TestMethod]
	public async Task AddAndRemoveMultiple()
	{
		var tag1 = _repository.Save(CreateEntry.Tag("vocarock"));

		await AddSongTags(_entry.Id, new TagBaseContract { Id = tag1.Id });

		var tags = new[] {
			new TagBaseContract { Name = "power metal" }
		};

		await AddSongTags(_entry.Id, tags);

		var entryTags = _entry.Tags.Tags.ToArray();
		entryTags.Length.Should().Be(1, "Number of tags");
		entryTags.Any(t => t.DefaultName == "power metal").Should().BeTrue("power metal tag is added");
		entryTags[0].UsageCount.Should().Be(1, "Number of usages for the added tag");
		tag1.UsageCount.Should().Be(0, "Number of usages for the removed tag");
	}

	[TestMethod]
	public void RemoveTagUsage()
	{
		var tag = _repository.Save(CreateEntry.Tag("rock"));
		var tag2 = _repository.Save(CreateEntry.Tag("metal"));
		var usage = _repository.Save(_entry.AddTag(tag).Result);
		_repository.Save(_entry.AddTag(tag2).Result);

		_queries.RemoveTagUsage<SongTagUsage, Song>(usage.Id, _repository.OfType<Song>());

		_entry.Tags.Usages.Count.Should().Be(1, "Number of tag usages for entry");
		tag.UsageCount.Should().Be(0, "Number of usages for tag");
		_entry.Tags.HasTag(tag).Should().BeFalse("Tag was removed from entry");
	}

	[TestMethod]
	public async Task AddTag_SendNotifications()
	{
		var followingUser = _repository.Save(CreateEntry.User(name: "Rin"));
		var tag = _repository.Save(CreateEntry.Tag("rock"));
		_repository.Save(followingUser.AddTag(tag));

		await AddSongTags(_entry.Id, Contract(tag.Id));

		var message = _repository.List<UserMessage>().FirstOrDefault();
		message.Should().NotBeNull("Message was sent");
		message?.User.Should().Be(followingUser, "User as expected");
	}

	[TestMethod]
	public async Task AddTag_SendNotifications_IgnoreSelf()
	{
		var tag = _repository.Save(CreateEntry.Tag("rock"));
		_repository.Save(_user.AddTag(tag));

		await AddSongTags(_entry.Id, Contract(tag.Id));

		_repository.List<UserMessage>().Count.Should().Be(0, "No message was sent");
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
	public async Task SkipInvalidTarget()
	{
		_existingTag.Targets = TagTargetTypes.Album;
		var tag = _repository.Save(CreateEntry.Tag("vocarock", 39));

		await AddSongTags(_entry.Id, Contract(_existingTag.Id), Contract(tag.Id));

		var entryTags = _entry.Tags.Tags.ToArray();
		entryTags.Length.Should().Be(1, "Number of tags");
		entryTags.Any(t => t.Id == 39).Should().BeTrue("vocarock tag is added");
	}
}
