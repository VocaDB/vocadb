namespace VocaDb.Model.Domain.Tags {

	public class RelatedTag {

		private Tag tag1;
		private Tag tag2;

		public RelatedTag() { }

		public RelatedTag(Tag tag1, Tag tag2) {
			Tag1 = tag1;
			Tag2 = tag2;
		}

		public int Id { get; set; }

		public Tag Tag1 {
			get { return tag1; }
			set {
				ParamIs.NotNull(() => value);
				tag1 = value;
			}
		}

		public Tag Tag2 {
			get { return tag2; }
			set {
				ParamIs.NotNull(() => value);
				tag2 = value;
			}
		}

		protected bool Equals(RelatedTag other) {
			return Equals(tag1, other.tag1) && Equals(tag2, other.tag2);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RelatedTag) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((tag1 != null ? tag1.GetHashCode() : 0)*397) ^ (tag2 != null ? tag2.GetHashCode() : 0);
			}
		}

	}

}
