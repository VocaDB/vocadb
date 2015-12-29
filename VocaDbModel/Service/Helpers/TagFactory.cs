using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Queries;

namespace VocaDb.Model.Service.Helpers {

	public class TagFactoryRepository : ITagFactory {

		private readonly AgentLoginData loginData;
		private readonly IDatabaseContext<Tag> ctx;

		public TagFactoryRepository(IDatabaseContext<Tag> ctx, AgentLoginData loginData) {
			this.ctx = ctx;
			this.loginData = loginData;
		}

		public Tag CreateTag(string englishName) {

			var tag = new Tag(new LocalizedString(englishName, ContentLanguageSelection.English));
			ctx.Save(tag);

			var archived = tag.CreateArchivedVersion(new TagDiff(), loginData, EntryEditEvent.Created, string.Empty);
			ctx.Save(archived);

			var activityEntry = new TagActivityEntry(tag, EntryEditEvent.Created, loginData.User, archived);
			new ActivityEntryQueries(ctx.OfType<ActivityEntry>(), null).AddActivityfeedEntry(activityEntry);

			return tag;

		}

	}

}
