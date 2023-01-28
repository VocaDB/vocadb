#nullable disable

using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.ReleaseEvents;

public class EventSeriesTagVote : GenericTagVote<EventSeriesTagUsage>
{
	public EventSeriesTagVote() { }
	public EventSeriesTagVote(EventSeriesTagUsage usage, User user) : base(usage, user) { }
}
