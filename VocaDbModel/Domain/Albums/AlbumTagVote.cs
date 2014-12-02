using System;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumTagVote : TagVote {

		private AlbumTagUsage tagUsage;

		public AlbumTagVote() { }

		public AlbumTagVote(AlbumTagUsage usage, User user)
			: base(user) {

			Usage = usage;

		}

		public virtual AlbumTagUsage Usage {
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
