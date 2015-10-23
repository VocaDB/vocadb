using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Helpers {

	public class TagFactoryRepository : ITagFactory {

		private readonly AgentLoginData loginData;
		private readonly IRepositoryContext<Tag> ctx;

		public TagFactoryRepository(IRepositoryContext<Tag> ctx, AgentLoginData loginData) {
			this.ctx = ctx;
			this.loginData = loginData;
		}

		public Tag CreateTag(string name) {

			var tag = new Tag(name);
			ctx.Save(tag);

			var archived = tag.CreateArchivedVersion(new TagDiff(), loginData, EntryEditEvent.Created);
			ctx.Save(archived);

			var activityEntry = new TagActivityEntry(tag, EntryEditEvent.Created, loginData.User, archived);
			new ActivityEntryQueries(ctx.OfType<ActivityEntry>(), null).AddActivityfeedEntry(activityEntry);

			return tag;

		}

	}

}
