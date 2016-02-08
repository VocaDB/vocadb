using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesWithEventsContract : ReleaseEventSeriesContract {

		public ReleaseEventSeriesWithEventsContract() {}

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference)
			: base(series) {

			Events = series.Events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e)).ToArray();
		
		}

		public ReleaseEventSeriesWithEventsContract(ReleaseEventSeries series, IEnumerable<ReleaseEvent> events, ContentLanguagePreference languagePreference)
			: base(series) {

			ParamIs.NotNull(() => events);

			Events = events.OrderBy(e => e.SeriesNumber).ThenBy(e => e.Date.DateTime).Select(e => new ReleaseEventContract(e)).ToArray();

		}

		public ReleaseEventContract[] Events { get; set; }

	}
}
