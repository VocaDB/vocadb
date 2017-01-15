
using System;

namespace VocaDb.Model.Domain {

	public abstract class EntryHit {

		private int agent;

		protected EntryHit() { }

		protected EntryHit(int agent) {
			Agent = agent;
		}

		public virtual int Agent {
			get { return agent; }
			set { agent = value; }
		}

		/// <summary>
		/// Database-generated
		/// </summary>
		public virtual DateTime Date { get; set; }

		public virtual long Id { get; set; }

	}

	public class GenericEntryHit<TEntry> : EntryHit where TEntry: class {
		private TEntry entry;

		public GenericEntryHit() {}
		public GenericEntryHit(TEntry entry, int agent) : base(agent) {
			Entry = entry;
		}

		public virtual TEntry Entry {
			get { return entry; }
			set {
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public override string ToString() {
			return "Hit for " + Entry + " by " + Agent;
		}

	}

}
