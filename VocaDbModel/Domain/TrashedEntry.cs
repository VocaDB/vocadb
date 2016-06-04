using System;
using System.Xml.Linq;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain {

	public class TrashedEntry {

		private XDocument data;
		private string name;
		private string notes;
		private User user;

		public TrashedEntry() {
			Created = DateTime.Now;
			Notes = string.Empty;
		}

		public TrashedEntry(IEntryBase entry, XDocument data, User user, string notes = "")
			: this() {

			ParamIs.NotNull(() => entry);

			Data = data;
			EntryId = entry.Id;
			EntryType = entry.EntryType;
			Name = entry.DefaultName;
			User = user;
			Notes = notes;

		}

		public virtual DateTime Created { get; set; }

		public virtual XDocument Data {
			get { return data; }
			set { 
				ParamIs.NotNull(() => value);
				data = value; 
			}
		}

		/// <summary>
		/// ID of the entry that was deleted.
		/// </summary>
		public virtual int EntryId { get; set; }

		public virtual EntryType EntryType { get; set; }

		public virtual int Id { get; set; }

		/// <summary>
		/// Default name of the entry that was deleted. Cannot be null or empty.
		/// </summary>
		public virtual string Name {
			get { return name; }
			set {
                ParamIs.NotNullOrEmpty(() => value);
			    name = value;
			}
		}

		public virtual string Notes {
			get { return notes; }
			protected set {
				ParamIs.NotNull(() => value);
				notes = value;
			}
		}

		/// <summary>
		/// User who deleted the entry. Cannot be null.
		/// </summary>
		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value; 
			}
		}
	}

}
