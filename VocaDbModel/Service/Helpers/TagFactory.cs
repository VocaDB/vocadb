#nullable disable

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
		private readonly AgentLoginData _loginData;
		private readonly IDatabaseContext<Tag> _ctx;

		public TagFactoryRepository(IDatabaseContext<Tag> ctx, AgentLoginData loginData)
		{
			this._ctx = ctx;
			this._loginData = loginData;
		}

		public async Task<Tag> CreateTagAsync(string englishName)
		{
			var tag = new Tag(new LocalizedString(englishName, ContentLanguageSelection.English));
			await _ctx.SaveAsync(tag);

			var archived = ArchivedTagVersion.Create(tag, new TagDiff(), _loginData, EntryEditEvent.Created, string.Empty);
			await _ctx.SaveAsync(archived);

			var activityEntry = new TagActivityEntry(tag, EntryEditEvent.Created, _loginData.User, archived);
			await new ActivityEntryQueries(_ctx.OfType<ActivityEntry>(), null).AddActivityfeedEntryAsync(activityEntry);

			return tag;
		}
	}
}
