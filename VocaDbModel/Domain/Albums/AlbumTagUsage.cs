using System.Linq;
using System.Collections.Generic;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumTagUsage : TagUsage {

		private Album album;
		private IList<AlbumTagVote> votes = new List<AlbumTagVote>();

		public AlbumTagUsage() { }

		public AlbumTagUsage(Album album, Tag tag)
			: base(tag) {

			Album = album;

		}

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public override IEntryBase Entry {
			get { return Album; }
		}

		public virtual IList<AlbumTagVote> Votes {
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

			var vote = new AlbumTagVote(this, user);
			Votes.Add(vote);
			Count++;

			return vote;

		}

		public override void Delete() {

			Album.Tags.Usages.Remove(this);
			Tag.AllAlbumTagUsages.Remove(this);
			Votes.Clear();
			Count = 0;

		}

		public virtual AlbumTagVote FindVote(User user) {

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
