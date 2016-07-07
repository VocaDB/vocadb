using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ReleaseEventSeriesWebLink : GenericWebLink<ReleaseEventSeries> {

		public ReleaseEventSeriesWebLink() { }

		public ReleaseEventSeriesWebLink(ReleaseEventSeries series, string description, string url, WebLinkCategory category)
			: base(series, description, url, category) { }

	}

}
