#nullable disable

using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs
{
	public class SongListTagVote : GenericTagVote<SongListTagUsage>
	{
		public SongListTagVote() { }

		public SongListTagVote(SongListTagUsage usage, User user) : base(usage, user) { }
	}
}
