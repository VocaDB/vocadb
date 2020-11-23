using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.ReleaseEvents
{

	public class EventSeriesTagUsage : GenericTagUsage<ReleaseEventSeries, EventSeriesTagVote>
	{

		public EventSeriesTagUsage() { }

		public EventSeriesTagUsage(ReleaseEventSeries song, Tag tag) : base(song, tag) { }

		public override TagVote CreateVote(User user)
		{

			if (FindVote(user) != null)
				return null;

			var vote = new EventSeriesTagVote(this, user);
			Votes.Add(vote);
			Count++;

			return vote;

		}

		public override void Delete()
		{

			base.Delete();

			Entry.Tags.Usages.Remove(this);
			Tag.AllEventSeriesTagUsages.Remove(this);

		}

		public override TagUsage Move(Tag target)
		{

			ParamIs.NotNull(() => target);

			if (target.Equals(Tag))
				return this;

			// TODO: have to make a clone because of NH reparenting issues, see http://stackoverflow.com/questions/28114508/nhibernate-change-parent-deleted-object-would-be-re-saved-by-cascade
			Tag.AllEventSeriesTagUsages.Remove(this);
			Entry.Tags.Usages.Remove(this);

			var newUsage = new EventSeriesTagUsage(Entry, target);
			target.AllEventSeriesTagUsages.Add(newUsage);
			Entry.Tags.Usages.Add(newUsage);

			return newUsage;

		}

	}

}
