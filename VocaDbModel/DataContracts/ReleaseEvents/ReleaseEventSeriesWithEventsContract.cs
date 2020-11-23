using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ReleaseEventSeriesWithEventsContract : ReleaseEventSeriesContract
	{
		public ReleaseEventSeriesWithEventsContract() { }

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference)
			: base(series, languagePreference)
		{
			Events = series.Events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e, languagePreference)).ToArray();
		}

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series, IEnumerable<ReleaseEvent> events, ContentLanguagePreference languagePreference)
			: base(series, languagePreference)
		{
			ParamIs.NotNull(() => events);

			Events = events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e, languagePreference)).ToArray();
		}

		public ReleaseEventContract[] Events { get; set; }
	}
}
