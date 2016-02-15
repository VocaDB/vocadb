using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Domain.ExtLinks {

	public abstract class GenericWebLink<TEntry> : WebLink where TEntry : class {

		private TEntry entry;

		protected GenericWebLink() { }

		protected GenericWebLink(TEntry entry, WebLinkContract contract)
			: base(contract) {

			Entry = entry;

		}

		public virtual TEntry Entry {
			get { return entry; }
			set {
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public virtual bool Equals(GenericWebLink<TEntry> another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as GenericWebLink<TEntry>);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override string ToString() {
			return string.Format("{0} for {1}", base.ToString(), Entry);
		}


	}

}
