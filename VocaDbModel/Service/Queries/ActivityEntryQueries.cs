using System.Linq;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Service.Queries {

	public class ActivityEntryQueries {

		private readonly IRepositoryContext<ActivityEntry> ctx;
		private readonly IUserPermissionContext permissionContext;

		public ActivityEntryQueries(IRepositoryContext<ActivityEntry> ctx, IUserPermissionContext permissionContext) {
			this.ctx = ctx;
			this.permissionContext = permissionContext;
		}

		public void AddActivityfeedEntry(ActivityEntry entry) {

			var latestEntries = ctx.Query()
				.OrderByDescending(a => a.CreateDate)
				.Take(10)	// time cutoff would be better instead of an arbitrary number of activity entries
				.ToArray();

			if (latestEntries.Any(e => e.IsDuplicate(entry)))
				return;

			ctx.Save(entry);

		}

		public void AddEntryEditedEntry(Tag entry, EntryEditEvent editEvent, ArchivedTagVersion archivedVersion) {

			var user = ctx.OfType<User>().GetLoggedUser(permissionContext);
			var activityEntry = new TagActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(activityEntry);

		}

	}

}
