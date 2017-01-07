using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

namespace VocaDb.Tests.Service.Queries {

	[TestClass]
	public class TagUsageQueriesTests {

		private Song entry;
		private readonly FakeUserRepository repository = new FakeUserRepository();
        private TagUsageQueries queries;
		private User user;

		private void AddTags(int entryId, params TagBaseContract[] tags) {

			queries.AddTags<Song, SongTagUsage>(entryId, tags, false, repository, new FakeEntryLinkFactory(), new EnumTranslations(),
				song => song.Tags, (song, ctx) => new SongTagUsageFactory(ctx, song));

		}

		private TagBaseContract Contract(int id) {
			return new TagBaseContract { Id = id };
		}

		private TagBaseContract Contract(string name) {
			return new TagBaseContract { Name = name };
		}

		[TestInitialize]
		public void SetUp() {
			user = repository.Save(CreateEntry.User(group: UserGroupId.Trusted));
			queries = new TagUsageQueries(new FakePermissionContext(user));
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
			Assert.AreEqual(1, usage.Tag.UsageCount, "Number of usages for tag");

			Assert.AreEqual(2, repository.List<Tag>().Count, "Number of tags in the repository");
			Assert.IsTrue(repository.List<Tag>().Contains(usage.Tag), "Tag in repository is the same as the one applied");

		}

		[TestMethod]
		public void AddExistingTagByName() {

			repository.Save(CreateEntry.Tag("vocarock", 39));

			AddTags(entry.Id, Contract("vocarock"));

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
			Assert.AreEqual(1, tag.UsageCount, "Number of usages for tag");

		}

		/// <summary>
		/// Add tag based on translated name
		/// </summary>
		[TestMethod]
		public void AddTagByTranslation() {

			var tag = repository.Save(CreateEntry.Tag("rock"));
			tag.CreateName("ロック", ContentLanguageSelection.Japanese);

			AddTags(entry.Id, Contract("ロック"));

			Assert.AreEqual(1, entry.Tags.Tags.Count(), "Number of tags");
			var usage = entry.Tags.Usages.First();
			Assert.AreEqual("rock", usage.Tag.DefaultName, "Added tag name");
			Assert.AreEqual(tag.Id, usage.Tag.Id, "Added tag Id");

		}

		/// <summary>
		/// Add renamed tag by name
		/// </summary>
		[TestMethod]
		public void AddNewTag_TagIsRenamed() {

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

			var tag = repository.Save(CreateEntry.Tag("rock"));
			tag.CreateName("ロック", ContentLanguageSelection.Japanese);

			var tags = new[] {
				Contract(tag.Id),
				Contract(tag.DefaultName),
				Contract("ロック")
			};

			AddTags(entry.Id, tags);

			Assert.AreEqual(1, entry.Tags.Tags.Count(), "Number of tags");
			var usage = entry.Tags.Usages.First();
			Assert.AreEqual(tag.Id, usage.Tag.Id, "Added tag name");
			Assert.AreEqual(1, tag.UsageCount, "Number of usages for tag");

		}

		[TestMethod]
		public void AddMultiple() {

			var tag1 = repository.Save(CreateEntry.Tag("vocarock"));

			var tags = new[] {
				new TagBaseContract { Id = tag1.Id },
				new TagBaseContract { Name = "power metal" }
			};

			AddTags(entry.Id, tags);

			var entryTags = entry.Tags.Tags.ToArray();
            Assert.AreEqual(2, entryTags.Length, "Number of applied tags");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "vocarock"), "vocarock tag is added");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "power metal"), "power metal tag is added");
			Assert.AreEqual(1, entryTags[0].UsageCount, "Number of usages for tag");

		}

		[TestMethod]
		public void AddAndRemoveMultiple() {

			var tag1 = repository.Save(CreateEntry.Tag("vocarock"));

			AddTags(entry.Id, new TagBaseContract { Id = tag1.Id });

			var tags = new[] {
				new TagBaseContract { Name = "power metal" }
			};

			AddTags(entry.Id, tags);

			var entryTags = entry.Tags.Tags.ToArray();
			Assert.AreEqual(1, entryTags.Length, "Number of tags");
			Assert.IsTrue(entryTags.Any(t => t.DefaultName == "power metal"), "power metal tag is added");
			Assert.AreEqual(1, entryTags[0].UsageCount, "Number of usages for the added tag");
			Assert.AreEqual(0, tag1.UsageCount, "Number of usages for the removed tag");

		}

		[TestMethod]
		public void RemoveTagUsage() {

			var tag = repository.Save(CreateEntry.Tag("rock"));
			var tag2 = repository.Save(CreateEntry.Tag("metal"));
			var usage = repository.Save(entry.AddTag(tag).Result);
			repository.Save(entry.AddTag(tag2).Result);

			queries.RemoveTagUsage<SongTagUsage, Song>(usage.Id, repository.OfType<Song>());

			Assert.AreEqual(1, entry.Tags.Usages.Count, "Number of tag usages for entry");
			Assert.AreEqual(0, tag.UsageCount, "Number of usages for tag");
			Assert.IsFalse(entry.Tags.HasTag(tag), "Tag was removed from entry");

		}

		[TestMethod]
		public void AddTag_SendNotifications() {

			var followingUser = repository.Save(CreateEntry.User(name: "Rin"));
			var tag = repository.Save(CreateEntry.Tag("rock"));
			repository.Save(followingUser.AddTag(tag));

			AddTags(entry.Id, Contract(tag.Id));

			var message = repository.List<UserMessage>().FirstOrDefault();
			Assert.IsNotNull("message", "Message was sent");
			Assert.AreEqual(followingUser, message?.User, "User as expected");

		}

		[TestMethod]
		public void AddTag_SendNotifications_IgnoreSelf() {

			var tag = repository.Save(CreateEntry.Tag("rock"));
			repository.Save(user.AddTag(tag));

			AddTags(entry.Id, Contract(tag.Id));

			Assert.AreEqual(0, repository.List<UserMessage>().Count, "No message was sent");

		}

	}

}
