#nullable disable

using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Queries
{
	public class ActivityEntryQueries
	{
		private readonly IDatabaseContext<ActivityEntry> _ctx;
		private readonly IUserPermissionContext _permissionContext;

		public ActivityEntryQueries(IDatabaseContext<ActivityEntry> ctx, IUserPermissionContext permissionContext)
		{
			_ctx = ctx;
			_permissionContext = permissionContext;
		}

		public void AddActivityfeedEntry(ActivityEntry entry)
		{
			var latestEntries = _ctx.Query()
				.OrderByDescending(a => a.CreateDate)
				.Take(10)   // time cutoff would be better instead of an arbitrary number of activity entries
				.ToArray();

			if (latestEntries.Any(e => e.IsDuplicate(entry)))
				return;

			_ctx.Save(entry);
		}

		public async Task AddActivityfeedEntryAsync(ActivityEntry entry)
		{
			var latestEntries = await _ctx.Query()
				.OrderByDescending(a => a.CreateDate)
				.Take(10)   // time cutoff would be better instead of an arbitrary number of activity entries
				.VdbToListAsync();

			if (latestEntries.Any(e => e.IsDuplicate(entry)))
				return;

			await _ctx.SaveAsync(entry);
		}

		public void AddEntryEditedEntry(Tag entry, EntryEditEvent editEvent, ArchivedTagVersion archivedVersion)
		{
			var user = _ctx.OfType<User>().GetLoggedUser(_permissionContext);
			var activityEntry = new TagActivityEntry(entry, editEvent, user, archivedVersion);
			AddActivityfeedEntry(activityEntry);
		}
	}
}
