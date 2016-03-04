using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.Domain.Tags {

	/// <summary>
	/// Tests for <see cref="TagManager{T}"/>.
	/// </summary>
	[TestClass]
	public class TagManagerTests {

		class TagFactory : ITagFactory, ITagUsageFactory<SongTagUsage> {

			private readonly Song song = new Song();

			public Tag CreateTag(string name) {
				return new Tag(name);
			}

			public SongTagUsage CreateTagUsage(Tag tag) {
				return new SongTagUsage(song, tag);
			}

			public SongTagUsage CreateTagUsage(Tag tag, SongTagUsage usage) {
				return new SongTagUsage(usage.Song, tag);
			}

		}

		private Tag aliasedTag;
		private TagManager<SongTagUsage> manager; 
		private Tag tag;
		private TagFactory tagFactory;
		private User user;

		private void SyncVotes(params Tag[] tags) {
			manager.SyncVotes(user, tags, tagFactory);
		}

		[TestInitialize]
		public void SetUp() {
			
			tagFactory = new TagFactory();
			tag = new Tag("drumnbass");
			aliasedTag = new Tag("Drum_and_bass") { AliasedTo = tag };
			manager = new TagManager<SongTagUsage>();
			user = new User();

		}

		[TestMethod]
		public void Sync_ReplaceWithAlias() {

			SyncVotes(aliasedTag);

			Assert.AreEqual(1, manager.Tags.Count(), "one tag");
			Assert.AreEqual(tag, manager.Tags.First(), "tag is the actual one");

		}

		[TestMethod]
		public void Sync_BothAliasAndActual() {

			SyncVotes(tag, aliasedTag);

			Assert.AreEqual(1, manager.Tags.Count(), "one tag");
			Assert.AreEqual(tag, manager.Tags.First(), "tag is the actual one");

		}

	}

}
