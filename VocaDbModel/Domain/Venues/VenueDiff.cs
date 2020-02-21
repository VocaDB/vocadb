using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Venues {

	public class VenueDiff : EntryDiff<VenueEditableFields> {

		public VenueDiff() : base(true) { }
		public VenueDiff(bool isSnapshot) : base(isSnapshot) { }
		public VenueDiff(VenueEditableFields changedFields) : base(changedFields) { }

	}

}
