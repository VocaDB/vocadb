using System;

namespace VocaDb.Model.Domain.Artists {

	public class GroupForArtist {

		private Artist group;
		private Artist member;

		public GroupForArtist() { }

		public GroupForArtist(Artist group, Artist member) {

			Group = group;
			Member = member;

		}

		//public DateTime? BeginDate { get; set; }

		//public DateTime? EndDate { get; set; }

		public virtual Artist Group {
			get { return group; }
			set {
				ParamIs.NotNull(() => value);
				group = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual Artist Member {
			get { return member; }
			set {
				ParamIs.NotNull(() => value);
				member = value;
			}
		}

		public virtual void Delete() {

			Group.AllMembers.Remove(this);
			Member.AllGroups.Remove(this);

		}

		public virtual bool Equals(GroupForArtist another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as GroupForArtist);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public virtual void MoveToGroup(Artist target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Group))
				return;

			Group.AllMembers.Remove(this);
			Group = target;
			target.AllMembers.Add(this);

		}

		public virtual void MoveToMember(Artist target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Member))
				return;

			Member.AllGroups.Remove(this);
			Member = target;
			target.AllGroups.Add(this);

		}

	}
}
