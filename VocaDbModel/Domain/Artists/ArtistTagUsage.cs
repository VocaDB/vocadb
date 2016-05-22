using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistTagUsage : TagUsage {

		private Artist artist;
		private IList<ArtistTagVote> votes = new List<ArtistTagVote>();

		public ArtistTagUsage() { }

		public ArtistTagUsage(Artist artist, Tag tag)
			: base(tag) {

				Artist = artist;

		}

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public override IEntryBase Entry {
			get { return Artist; }
		}

		public virtual IList<ArtistTagVote> Votes {
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

			var vote = new ArtistTagVote(this, user);
			Votes.Add(vote);
			Count++;

			return vote;

		}

		public override void Delete() {

			base.Delete();

			Artist.Tags.Usages.Remove(this);
			Tag.AllArtistTagUsages.Remove(this);
			Votes.Clear();

		}

		public virtual ArtistTagVote FindVote(User user) {

			return Votes.FirstOrDefault(v => v.User.Equals(user));

		}

		public override TagUsage Move(Tag target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Tag))
				return this;

			// TODO: have to make a clone because of NH reparenting issues, see http://stackoverflow.com/questions/28114508/nhibernate-change-parent-deleted-object-would-be-re-saved-by-cascade
			Tag.AllArtistTagUsages.Remove(this);
			Artist.Tags.Usages.Remove(this);

			var newUsage = new ArtistTagUsage(Artist, target);
			target.AllArtistTagUsages.Add(newUsage);
			Artist.Tags.Usages.Add(newUsage);

			return newUsage;

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
