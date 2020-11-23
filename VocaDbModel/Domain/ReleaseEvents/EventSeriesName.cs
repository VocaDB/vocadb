using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public class EventSeriesName : EntryName<ReleaseEventSeries>
	{
		public EventSeriesName() { }

		public EventSeriesName(ReleaseEventSeries series, ILocalizedString localizedString) : base(series, localizedString) { }
	}
}
