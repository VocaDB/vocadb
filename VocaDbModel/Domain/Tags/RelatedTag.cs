namespace VocaDb.Model.Domain.Tags
{

	public class RelatedTag : IEntryWithIntId
	{

		private Tag ownerTag;
		private Tag linkedTag;

		public RelatedTag() { }

		public RelatedTag(Tag ownerTag, Tag linkedTag)
		{

			ParamIs.NotNull(() => ownerTag);
			ParamIs.NotNull(() => linkedTag);

			OwnerTag = ownerTag;
			LinkedTag = linkedTag;

		}

		public virtual int Id { get; set; }

		public virtual Tag OwnerTag
		{
			get { return ownerTag; }
			set
			{
				ParamIs.NotNull(() => value);
				ownerTag = value;
			}
		}

		public virtual Tag LinkedTag
		{
			get { return linkedTag; }
			set
			{
				ParamIs.NotNull(() => value);
				linkedTag = value;
			}
		}

		public virtual RelatedTag CreateReversed()
		{
			return new RelatedTag(LinkedTag, OwnerTag);
		}

		public virtual void Delete()
		{
			OwnerTag.RelatedTags.Remove(this);
			LinkedTag.RelatedTags.Remove(CreateReversed());
		}

		protected bool Equals(RelatedTag other)
		{
			return OwnerTag.Equals(other.OwnerTag) && LinkedTag.Equals(other.LinkedTag);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((RelatedTag)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((OwnerTag != null ? OwnerTag.GetHashCode() : 0) * 397) ^ (LinkedTag != null ? LinkedTag.GetHashCode() : 0);
			}
		}

	}

}
