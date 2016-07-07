using System.Linq;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesForEditContract : ReleaseEventSeriesContract {

		public ReleaseEventSeriesForEditContract() {
			Aliases = new string[] {};
			WebLinks = new WebLinkContract[0];
		}

		public ReleaseEventSeriesForEditContract(ReleaseEventSeries series) : base(series) {

			Aliases = series.Aliases.Select(a => a.Name).ToArray();
			WebLinks = series.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		public string[] Aliases { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}

}
