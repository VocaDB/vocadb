#nullable disable

using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs
{
	public class SongTagVote : GenericTagVote<SongTagUsage>
	{
		public SongTagVote() { }
		public SongTagVote(SongTagUsage usage, User user) : base(usage, user) { }
	}
}
