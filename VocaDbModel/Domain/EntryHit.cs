
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

		public virtual long Id { get; set; }

	}

}
