using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Domain.Comments
{
	public class GenericComment<T> : Comment where T : class, IEntryWithNames
	{
		private T entry;

		public GenericComment() { }

		public GenericComment(T entry, string message, AgentLoginData loginData)
			: base(message, loginData)
		{
			EntryForComment = entry;
		}

		public override IEntryWithNames Entry => EntryForComment;

		public virtual T EntryForComment
		{
			get => entry;
			set
			{
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public virtual bool Equals(GenericComment<T> another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as GenericComment<T>);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("comment [{0}] for " + Entry, Id);
		}
	}
}
