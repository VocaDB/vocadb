#nullable disable

using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.Domain.Tags
{
	/// <summary>
	/// Tests for <see cref="TagManager{T}"/>.
	/// </summary>
	[TestClass]
	public class TagManagerTests
	{
		class TagFactory : ITagFactory, ITagUsageFactory<SongTagUsage>
		{
			private readonly Song _song = new();

			public Task<Tag> CreateTagAsync(string name)
			{
				return Task.FromResult(new Tag(name));
			}

			public SongTagUsage CreateTagUsage(Tag tag)
			{
				return new SongTagUsage(_song, tag);
			}

			public SongTagUsage CreateTagUsage(Tag tag, SongTagUsage usage)
			{
				return new SongTagUsage(usage.Entry, tag);
			}
		}

		private TagManager<SongTagUsage> _manager;
		private Tag _tag;
		private TagFactory _tagFactory;
		private User _user;

		private void SyncVotes(params Tag[] tags)
		{
			_manager.SyncVotes(_user, tags, _tagFactory);
		}

		[TestInitialize]
		public void SetUp()
		{
			_tagFactory = new TagFactory();
			_tag = new Tag("drumnbass");
			_manager = new TagManager<SongTagUsage>();
			_user = new User();
		}
	}
}
