using System;

namespace VocaDb.Model.Domain.Artists
{
	public class ArtistForArtist : IEntryWithIntId
	{
		private Artist parent;
		private Artist member;

		public ArtistForArtist() { }

		public ArtistForArtist(Artist group, Artist member, ArtistLinkType linkType)
		{
			Parent = group;
			Member = member;
			LinkType = linkType;
		}

		public virtual Artist Parent
		{
			get { return parent; }
			set
			{
				ParamIs.NotNull(() => value);
				parent = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual ArtistLinkType LinkType { get; set; }

		public virtual Artist Member
		{
			get { return member; }
			set
			{
				ParamIs.NotNull(() => value);
				member = value;
			}
		}

		public virtual void Delete()
		{
			Parent.AllMembers.Remove(this);
			Member.AllGroups.Remove(this);
		}

		public virtual bool Equals(ArtistForArtist another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ArtistForArtist);
		}

		public virtual Artist GetArtist(LinkDirection direction)
		{
			return direction == LinkDirection.ManyToOne ? Parent : Member;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual void MoveToGroup(Artist target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Parent))
				return;

			Parent.AllMembers.Remove(this);
			Parent = target;
			target.AllMembers.Add(this);
		}

		public virtual void MoveToMember(Artist target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Member))
				return;

			Member.AllGroups.Remove(this);
			Member = target;
			target.AllGroups.Add(this);
		}

		public override string ToString()
		{
			return Parent + " for " + Member + " (" + LinkType + ")";
		}
	}
}
