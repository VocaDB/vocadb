using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.ReleaseEvents {
	public class EventTagUsage : GenericTagUsage<ReleaseEvent, EventTagVote> {

		public EventTagUsage() { }

		public EventTagUsage(ReleaseEvent song, Tag tag) : base(song, tag) { }

		public override TagVote CreateVote(User user) {

			if (FindVote(user) != null)
				return null;

			var vote = new EventTagVote(this, user);
			Votes.Add(vote);
			Count++;

			return vote;

		}

		public override void Delete() {

			base.Delete();

			Entry.Tags.Usages.Remove(this);
			Tag.AllEventTagUsages.Remove(this);

		}

		public override TagUsage Move(Tag target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Tag))
				return this;

			// TODO: have to make a clone because of NH reparenting issues, see http://stackoverflow.com/questions/28114508/nhibernate-change-parent-deleted-object-would-be-re-saved-by-cascade
			Tag.AllEventTagUsages.Remove(this);
			//Song.Tags.Usages.Remove(this);

			var newUsage = new EventTagUsage(Entry, target);
			target.AllEventTagUsages.Add(newUsage);
			//Song.Tags.Usages.Add(newUsage);

			return newUsage;

		}

	}
}
