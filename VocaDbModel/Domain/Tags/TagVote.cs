#nullable disable

using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Tags
{
	/// <summary>
	/// User's vote for a particular tag for a particular entry.
	/// </summary>
	public abstract class TagVote : IEntryWithLongId
	{
		private User _user;

		protected TagVote() { }

		protected TagVote(User user)
		{
			User = user;
		}

		public virtual long Id { get; set; }

		public abstract TagUsage UsageBase { get; }

		public virtual User User
		{
			get => _user;
			set
			{
				ParamIs.NotNull(() => value);
				_user = value;
			}
		}

		public virtual bool Equals(TagVote another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object obj) => Equals(obj as TagVote);

		public override int GetHashCode() => Id.GetHashCode();

		public override string ToString()
		{
			return $"Vote for {UsageBase} by {User}";
		}
	}
}
