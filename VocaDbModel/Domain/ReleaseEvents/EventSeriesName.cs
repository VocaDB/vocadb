using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class EventSeriesName : EntryName<ReleaseEventSeries> {

		public EventSeriesName() { }

		public EventSeriesName(ReleaseEventSeries series, LocalizedString localizedString) : base(series, localizedString) {}

	}

}
