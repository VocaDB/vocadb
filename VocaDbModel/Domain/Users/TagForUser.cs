using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Domain.Users
{
	/// <summary>
	/// User following a tag.
	/// </summary>
	public class TagForUser : IEntryWithIntId
	{
		private Tag tag;
		private User user;

		public TagForUser() { }

		public TagForUser(User user, Tag tag)
			: this()
		{
			User = user;
			Tag = tag;
		}

		public virtual int Id { get; set; }

		public virtual Tag Tag
		{
			get { return tag; }
			set
			{
				ParamIs.NotNull(() => value);
				tag = value;
			}
		}

		public virtual User User
		{
			get { return user; }
			set
			{
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} following {1}", User, Tag);
		}
	}
}
