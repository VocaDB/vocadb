using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Queries;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Queries {

	[TestClass]
	public class TagUsageQueriesTests {

		private Song entry;
		private readonly FakeUserRepository repository = new FakeUserRepository();
        private readonly TagUsageQueries queries = new TagUsageQueries();
		private User user;

		private void AddTags(int entryId, params TagBaseContract[] tags) {

			queries.AddTags<Song, SongTagUsage>(entryId, tags, false, repository, new FakePermissionContext(user),
				new FakeEntryLinkFactory(), song => song.Tags, (song, ctx) => new SongTagUsageFactory(ctx, song));

		}

		private TagBaseContract Contract(int id) {
			return new TagBaseContract { Id = id };
		}

		private TagBaseContract Contract(string name) {
			return new TagBaseContract { Name = name };
		}

		[TestInitialize]
		public void SetUp() {
			user = repository.Save(CreateEntry.User());
			entry = repository.Save(CreateEntry.Song(name: "Puppet"));
			repository.Save(CreateEntry.Tag("techno"));
		}

		[TestMethod]
		public void AddNewTagByName() {

			var tags = new[] { Contract("vocarock") };

			AddTags(entry.Id, tags);

			Assert.AreEqual(1, entry.Tags.Tags.Count(), "Number of tags");
			var usage = entry.Tags.Usages.First();
			Assert.AreEqual("vocarock", usage.Tag.DefaultName, "Added tag name");
			Assert.IsTrue(usage.HasVoteByUser(user), "Vote is by the logged in user");

			Assert.AreEqual(2, repository.List<Tag>().Count, "Number of tags in the repository");
			Assert.IsTrue(repository.List<Tag>().Contains(usage.Tag), "Tag in repository is the same as the one applied");

		}

		[TestMethod]
		public void AddExistingTagByName() {

			var tag = repository.Save(CreateEntry.Tag("vocarock", 39));
			tag.TranslatedName.Default = "rock";

			// vocarock was renamed to rock
			AddTags(entry.Id, Contract("rock"));

			var entryTags = entry.Tags.Tags.ToArray();
			Assert.AreEqual(1, entryTags.Length, "Number of tags");
			Assert.IsTrue(entryTags.Any(t => t.Id == 39), "vocarock tag is added");

		}

		[TestMethod]
		public void AddTagById() {

			var tag = repository.Save(CreateEntry.Tag("vocarock"));

			AddTags(entry.Id, Contract(tag.Id));

			Assert.AreEqual(1, entry.Tags.Tags.Count(), "Number of tags");
			var usage = entry.Tags.Usages.First();
			Assert.AreEqual("vocarock", usage.Tag.DefaultName, "Added tag name");
			Assert.AreEqual(tag.Id, usage.Tag.Id, "Added tag Id");

		}

		[TestMethod]
		public void AddAlias() {

			var parentTag = repository.Save(CreateEntry.Tag("vocarock"));
			var aliasTag = repository.Save(CreateEntry.Tag("rock"));
			aliasTag.AliasedTo = parentTag;

			// rock is aliased to vocarock, so vocarock gets added instead
			AddTags(entry.Id, Contract(aliasTag.Id));

			var entryTags = entry.Tags.Tags.ToArray();
			Assert.AreEqual(1, entryTags.Length, "Number of tags");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "vocarock"), "vocarock tag is added");

		}

		// Adding a new tag where tag name already exists as an ID, but display name is renamed.
		[TestMethod]
		public void AddNewTag_TagNameExists() {

			var tag = repository.Save(CreateEntry.Tag("vocarock", 39));
			tag.Names.First().Value = "rock";
			tag.Names.UpdateSortNames();

			// Attempting to add tag "vocarock". The "vocarock" tag was renamed as "rock" so this is a new tag.
			AddTags(entry.Id, Contract("vocarock"));

			var entryTags = entry.Tags.Tags.ToArray();
			Assert.AreEqual(1, entryTags.Length, "Number of tags");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "vocarock"), "vocarock tag is added");
			Assert.IsTrue(entryTags.Any(t => t.Id != 39), "tag name cannot be the same as rock tag");

		}

		[TestMethod]
		public void SkipDuplicates() {

			var tag = repository.Save(CreateEntry.Tag("vocarock"));

			var tags = new[] {
				new TagBaseContract { Id = tag.Id },
				new TagBaseContract { Name = tag.DefaultName }
			};

			AddTags(entry.Id, tags);

			Assert.AreEqual(1, entry.Tags.Tags.Count(), "Number of tags");
			var usage = entry.Tags.Usages.First();
			Assert.AreEqual(tag.Id, usage.Tag.Id, "Added tag name");

		}

		[TestMethod]
		public void AddMultiple() {

			var tag1 = repository.Save(CreateEntry.Tag("vocarock"));

			var tags = new[] {
				new TagBaseContract { Id = tag1.Id },
				new TagBaseContract { Name = "power_metal" }
			};

			AddTags(entry.Id, tags);

			var entryTags = entry.Tags.Tags.ToArray();
            Assert.AreEqual(2, entryTags.Length, "Number of applied tags");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "vocarock"), "vocarock tag is added");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "power_metal"), "power_metal tag is added");

		}

		[TestMethod]
		public void AddAndRemoveMultiple() {

			var tag1 = repository.Save(CreateEntry.Tag("vocarock"));

			AddTags(entry.Id, new TagBaseContract { Id = tag1.Id });

			var tags = new[] {
				new TagBaseContract { Name = "power_metal" }
			};

			AddTags(entry.Id, tags);

			var entryTags = entry.Tags.Tags.ToArray();
			Assert.AreEqual(1, entryTags.Length, "Number of tags");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "power_metal"), "power_metal tag is added");

		}
	}

}
