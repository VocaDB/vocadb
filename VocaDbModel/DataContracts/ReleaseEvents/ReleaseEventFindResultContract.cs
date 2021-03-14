#nullable disable

using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	/// <summary>
	/// Result of attempting to find <see cref="ReleaseEvent"/> based on name.
	/// May include the exact event or event series with intelligent guess of series number and suffix.
	/// </summary>
	public class ReleaseEventFindResultContract
	{
		public ReleaseEventFindResultContract() { }

#nullable enable
		public ReleaseEventFindResultContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => releaseEvent);

			EventId = releaseEvent.Id;
			EventName = releaseEvent.TranslatedName[languagePreference];
		}
#nullable disable

		public ReleaseEventFindResultContract(string eventName)
		{
			EventName = eventName;
		}

		public ReleaseEventFindResultContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference, int seriesNumber, string seriesSuffix, string eventName)
		{
			Series = new ReleaseEventSeriesContract(series, languagePreference);
			SeriesNumber = seriesNumber;
			SeriesSuffix = seriesSuffix;
			EventName = eventName;
		}

		public int EventId { get; init; }

		public string EventName { get; init; }

		public bool IsKnownEvent => EventId != 0;

		public ReleaseEventSeriesContract Series { get; init; }

		public int SeriesNumber { get; init; }

		public string SeriesSuffix { get; init; }
	}
}
