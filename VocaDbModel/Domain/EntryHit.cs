
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

}
