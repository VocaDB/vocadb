using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class PVForEvent : GenericPV<ReleaseEvent> {

		public PVForEvent() { }
		public PVForEvent(ReleaseEvent entry, PVContract contract) : base(entry, contract) { }

		public override void OnDelete() {
			Entry.PVs.Remove(this);
		}

	}

}
