using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventFindResultContract {

		public ReleaseEventFindResultContract() { }

		public ReleaseEventFindResultContract(ReleaseEvent releaseEvent) {

			ParamIs.NotNull(() => releaseEvent);

			EventId = releaseEvent.Id;
			EventName = releaseEvent.Name;

		}

		public ReleaseEventFindResultContract(string eventName) {

			EventName = eventName;

		}

		public ReleaseEventFindResultContract(ReleaseEventSeries series, int seriesNumber, string seriesSuffix, string eventName) {

			Series = new ReleaseEventSeriesContract(series);
			SeriesNumber = seriesNumber;
			SeriesSuffix = seriesSuffix;
			EventName = eventName;

		}

		public int EventId { get; set; }

		public string EventName { get; set; }

		public bool IsKnownEvent {
			get {
				return EventId != 0;
			}
		}

		public ReleaseEventSeriesContract Series { get; set; }

		public int SeriesNumber { get; set; }

		public string SeriesSuffix { get; set; }

	}

}
