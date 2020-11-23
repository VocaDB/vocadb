using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.ReleaseEvents
{

	public class EventTagVote : GenericTagVote<EventTagUsage>
	{

		public EventTagVote() { }
		public EventTagVote(EventTagUsage usage, User user) : base(usage, user) { }

	}

}
