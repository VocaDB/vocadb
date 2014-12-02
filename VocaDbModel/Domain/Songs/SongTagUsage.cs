using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public class SongTagUsage : TagUsage {

		private Song song;
		private IList<SongTagVote> votes = new List<SongTagVote>();

		public SongTagUsage() { }

		public SongTagUsage(Song song, Tag tag)
			: base(tag) {

			Song = song;

		}

		public virtual Song Song {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public override IEntryBase Entry {
			get { return Song; }
		}

		public virtual IList<SongTagVote> Votes {
			get { return votes; }
			set {
				ParamIs.NotNull(() => value);
				votes = value;
			}
		}

		public override IEnumerable<TagVote> VotesBase {
			get { return Votes; }
		}

		public override TagVote CreateVote(User user) {

			if (FindVote(user) != null)
				return null;

			var vote = new SongTagVote(this, user);
			Votes.Add(vote);
			Count++;

			return vote;

		}

		public override void Delete() {

			Song.Tags.Usages.Remove(this);
			Tag.AllSongTagUsages.Remove(this);
			Votes.Clear();
			Count = 0;

		}

		public virtual SongTagVote FindVote(User user) {

			return Votes.FirstOrDefault(v => v.User.Equals(user));

		}

		public override TagVote RemoveVote(User user) {

			var vote = FindVote(user);

			if (vote == null)
				return null;

			Votes.Remove(vote);
			Count--;

			return vote;

		}

	}
}
