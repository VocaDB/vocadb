using System;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Activityfeed
{
	/// <summary>
	/// Activity entries are primarily shown in the activity feed on the front page, 
	/// but they are also used for statistics such as the number of edits per user.
	/// They can be divided into two main operations: create and update.
	/// Entry-specific subclasses contain more detailed information about the activity.
	/// Consecutive activity entries by the same user for the same entry are merged.
	/// </summary>
	public abstract class ActivityEntry : IEntryWithIntId
	{
		private User author;

		protected ActivityEntry()
		{
			CreateDate = DateTime.Now;
		}

		protected ActivityEntry(User author, EntryEditEvent editEvent)
			: this()
		{
			Author = author;
			EditEvent = editEvent;
		}

		/// <summary>
		/// Archived entry version. Can be null.
		/// </summary>
		public abstract ArchivedObjectVersion ArchivedVersionBase { get; }

		public virtual User Author
		{
			get { return author; }
			set
			{
				ParamIs.NotNull(() => value);
				author = value;
			}
		}

		public virtual DateTime CreateDate { get; set; }

		public virtual EntryEditEvent EditEvent { get; set; }

		/// <summary>
		/// Entry. Cannot be null.
		/// </summary>
		public abstract IEntryWithNames EntryBase { get; }

		public abstract EntryType EntryType { get; }

		public virtual int Id { get; set; }

		public virtual bool IsDuplicate(ActivityEntry entry)
		{
			return (Author.Equals(entry.Author) && EntryBase.Equals(entry.EntryBase));
		}

		public override string ToString()
		{
			return string.Format("activity entry ({0}) for {1}", EditEvent, EntryBase);
		}
	}

	/// <summary>
	/// Common entry edit events
	/// </summary>
	public enum EntryEditEvent
	{
		Created = 1,

		Updated = 2,

		Deleted = 3,

		Restored = 4
	}
}
