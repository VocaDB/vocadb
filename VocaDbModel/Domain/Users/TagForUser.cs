#nullable disable

using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Domain.Users
{
	/// <summary>
	/// User following a tag.
	/// </summary>
	public class TagForUser : IEntryWithIntId
	{
		private Tag _tag;
		private User _user;

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
			get => _tag;
			set
			{
				ParamIs.NotNull(() => value);
				_tag = value;
			}
		}

		public virtual User User
		{
			get => _user;
			set
			{
				ParamIs.NotNull(() => value);
				_user = value;
			}
		}

#nullable enable
		public override string ToString()
		{
			return $"{User} following {Tag}";
		}
#nullable disable
	}
}
