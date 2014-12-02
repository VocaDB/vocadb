using System.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesForEditContract : ReleaseEventSeriesContract {

		public ReleaseEventSeriesForEditContract() {
			Aliases = new string[] {};
		}

		public ReleaseEventSeriesForEditContract(ReleaseEventSeries series) : base(series) {

			Aliases = series.Aliases.Select(a => a.Name).ToArray();

		}

		public string[] Aliases { get; set; }

	}

}
