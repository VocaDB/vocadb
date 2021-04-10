#nullable disable

using System;

namespace VocaDb.Model.Domain.Artists
{
	public class ArtistForArtist : IEntryWithIntId
	{
		private Artist _parent;
		private Artist _member;

		public ArtistForArtist() { }

		public ArtistForArtist(Artist group, Artist member, ArtistLinkType linkType)
		{
			Parent = group;
			Member = member;
			LinkType = linkType;
		}

		public virtual Artist Parent
		{
			get => _parent;
			set
			{
				ParamIs.NotNull(() => value);
				_parent = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual ArtistLinkType LinkType { get; set; }

		public virtual Artist Member
		{
			get => _member;
			set
			{
				ParamIs.NotNull(() => value);
				_member = value;
			}
		}

		public virtual void Delete()
		{
			Parent.AllMembers.Remove(this);
			Member.AllGroups.Remove(this);
		}

#nullable enable
		public virtual bool Equals(ArtistForArtist? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as ArtistForArtist);
		}
#nullable disable

		public virtual Artist GetArtist(LinkDirection direction)
		{
			return direction == LinkDirection.ManyToOne ? Parent : Member;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

#nullable enable
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
#nullable disable

		public override string ToString()
		{
			return Parent + " for " + Member + " (" + LinkType + ")";
		}
	}
}
