using System;
using System.Xml.Linq;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Activityfeed;

namespace VocaDb.Model.Domain.Versioning {

	public abstract class ArchivedObjectVersion {

		private string notes;

		protected ArchivedObjectVersion() {
			Created = DateTime.Now;
		}

		protected ArchivedObjectVersion(XDocument data, AgentLoginData author, int version, EntryStatus status, string notes)
			: this() {

			ParamIs.NotNull(() => author);

			Data = data;
			AgentName = author.Name;
			Author = author.User;
			Notes = notes;
			Status = status;
			Version = version;

		}

		public virtual string AgentName { get; protected set; }

		/// <summary>
		/// User who created this version. Can be null.
		/// </summary>
		public virtual User Author { get; protected set; }

		public virtual DateTime Created { get; protected set; }

		public virtual XDocument Data { get; protected set; }

		public abstract IEntryDiff DiffBase { get; }

		public abstract EntryEditEvent EditEvent { get; }

		public abstract IEntryWithNames EntryBase { get; }

		public virtual int Id { get; protected set; }

		public virtual string Notes {
			get { return notes; }
			protected set {
				ParamIs.NotNull(() => value);
				notes = value; 
			}
		}

		public virtual EntryStatus Status { get; protected set; }

		/// <summary>
		/// Version number. 
		/// </summary>
		/// <remarks>
		/// Note that not all entry types track version number. For those entry types this will always be 0.
		/// </remarks>
		public virtual int Version { get; protected set; }

		public override string ToString() {
			return string.Format("archived version {0} for {1}", Version, EntryBase);
		}

	}

}
