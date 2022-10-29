namespace VocaDb.Model.Domain.Users
{
	public class OldUsername : IEntryWithIntId
	{
#nullable disable
		public OldUsername()
		{
			DateUtc = DateTime.Now;
		}
#nullable enable

		public OldUsername(User user, string oldName) : this()
		{
			ParamIs.NotNull(() => user);
			ParamIs.NotNullOrEmpty(() => oldName);

			User = user;
			OldName = oldName;
		}

		public virtual DateTime DateUtc { get; set; }

		public virtual int Id { get; set; }

		public virtual string OldName { get; set; }

		public virtual User User { get; set; }
	}
}
