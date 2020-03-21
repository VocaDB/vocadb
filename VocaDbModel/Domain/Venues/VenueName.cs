using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.Venues {

	public class VenueName : EntryName<Venue> {

		public VenueName() { }

		public VenueName(Venue venue, ILocalizedString localizedString) : base(venue, localizedString) { }

	}

}
