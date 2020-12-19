#nullable disable

using System;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Comments
{
	/// <summary>
	/// Base class for comments.
	/// Comments can be added for entries such as albums and users.
	/// </summary>
	public abstract class Comment : ICommentWithEntry, IDeletableEntry
	{
		private string _authorName;
		private string _message;

		protected Comment()
		{
			Created = DateTime.Now;
		}

		protected Comment(string message, AgentLoginData loginData)
			: this()
		{
			ParamIs.NotNull(() => loginData);

			Message = message;
			Author = loginData.User;
			AuthorName = loginData.Name;
		}

		public virtual User Author { get; set; }

		public virtual string AuthorName
		{
			get => _authorName;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_authorName = value;
			}
		}

		public virtual DateTime Created { get; set; }

		public virtual bool Deleted { get; set; }

		/// <summary>
		/// Entry owning this comment. Cannot be null.
		/// </summary>
		public abstract IEntryWithNames Entry { get; }

		public virtual EntryType EntryType => Entry.EntryType;

		public virtual GlobalEntryId GlobalId => new GlobalEntryId(EntryType, Id);

		public virtual int Id { get; set; }

		public virtual string Message
		{
			get => _message;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_message = value;
			}
		}

		public virtual void Delete() => Deleted = true;

		public override string ToString() => $"comment [{Id}] for {Entry}";
	}

	public interface IComment : IEntryWithIntId
	{
		string AuthorName { get; }

		DateTime Created { get; }

		string Message { get; }
	}

	public interface ICommentWithEntry : IComment
	{
		IEntryWithNames Entry { get; }
	}
}
