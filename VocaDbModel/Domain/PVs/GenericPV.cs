using System;
using VocaDb.Model.DataContracts.PVs;

namespace VocaDb.Model.Domain.PVs {

	public abstract class GenericPV<TEntry> : PV where TEntry : class {

		private TEntry entry;

		protected GenericPV() {
		}

		protected GenericPV(TEntry entry, PVContract contract)
			: base(contract) {

			Entry = entry;
			Length = contract.Length;
			PublishDate = contract.PublishDate;

		}

		public virtual TEntry Entry {
			get => entry;
			set {
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		/// <summary>
		/// Length in seconds.
		/// </summary>
		public virtual int Length { get; set; }

		public virtual bool Equals(GenericPV<TEntry> another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as GenericPV<TEntry>);
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

		public override string ToString() {
			return string.Format("PV '{0}' on {1} [{2}] for {3}", PVId, Service, Id, Entry);
		}

	}

}
