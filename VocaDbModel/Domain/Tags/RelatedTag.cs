namespace VocaDb.Model.Domain.Tags {

	public class RelatedTag {

		private Tag ownerTag;
		private Tag linkedTag;

		public RelatedTag() { }

		public RelatedTag(Tag ownerTag, Tag linkedTag) {
			OwnerTag = ownerTag;
			LinkedTag = linkedTag;
		}

		public int Id { get; set; }

		public Tag OwnerTag {
			get { return ownerTag; }
			set {
				ParamIs.NotNull(() => value);
				ownerTag = value;
			}
		}

		public Tag LinkedTag {
			get { return linkedTag; }
			set {
				ParamIs.NotNull(() => value);
				linkedTag = value;
			}
		}

		protected bool Equals(RelatedTag other) {
			return Equals(ownerTag, other.ownerTag) && Equals(linkedTag, other.linkedTag);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RelatedTag) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((ownerTag != null ? ownerTag.GetHashCode() : 0)*397) ^ (linkedTag != null ? linkedTag.GetHashCode() : 0);
			}
		}

	}

}
