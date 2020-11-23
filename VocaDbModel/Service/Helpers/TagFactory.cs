using System.Threading.Tasks;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Queries;

namespace VocaDb.Model.Service.Helpers
{

	public class TagFactoryRepository : ITagFactory
	{

		private readonly AgentLoginData loginData;
		private readonly IDatabaseContext<Tag> ctx;

		public TagFactoryRepository(IDatabaseContext<Tag> ctx, AgentLoginData loginData)
		{
			this.ctx = ctx;
			this.loginData = loginData;
		}

		public async Task<Tag> CreateTagAsync(string englishName)
		{

			var tag = new Tag(new LocalizedString(englishName, ContentLanguageSelection.English));
			await ctx.SaveAsync(tag);

			var archived = ArchivedTagVersion.Create(tag, new TagDiff(), loginData, EntryEditEvent.Created, string.Empty);
			await ctx.SaveAsync(archived);

			var activityEntry = new TagActivityEntry(tag, EntryEditEvent.Created, loginData.User, archived);
			await new ActivityEntryQueries(ctx.OfType<ActivityEntry>(), null).AddActivityfeedEntryAsync(activityEntry);

			return tag;

		}
	}

}
