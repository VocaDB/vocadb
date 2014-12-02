using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Artists {
	public class ArtistTagVote : TagVote {

		private ArtistTagUsage tagUsage;

		public ArtistTagVote() { }

		public ArtistTagVote(ArtistTagUsage usage, User user)
			: base(user) {

			Usage = usage;

		}

		public virtual ArtistTagUsage Usage {
			get { return tagUsage; }
			set {
				ParamIs.NotNull(() => value);
				tagUsage = value;
			}
		}

		public override TagUsage UsageBase {
			get { return Usage; }
		}

	}

}
