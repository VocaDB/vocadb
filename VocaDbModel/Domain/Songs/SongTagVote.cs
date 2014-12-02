using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public class SongTagVote : TagVote {

		private SongTagUsage tagUsage;

		public SongTagVote() { }

		public SongTagVote(SongTagUsage usage, User user)
			: base(user) {

			Usage = usage;

		}

		public virtual SongTagUsage Usage {
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
