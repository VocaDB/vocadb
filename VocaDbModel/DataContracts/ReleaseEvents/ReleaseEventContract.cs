using System;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventContract : IEntryWithIntId {

		public ReleaseEventContract() {
			Description = string.Empty;
		}

		public ReleaseEventContract(ReleaseEvent ev, bool includeSeries = false)
			: this() {

			ParamIs.NotNull(() => ev);

			CustomName = ev.CustomName;
			Date = ev.Date;
			Description = ev.Description;
			Id = ev.Id;
			Name = ev.Name;

			if (includeSeries && ev.Series != null)
				Series = new ReleaseEventSeriesContract(ev.Series);

		}

		public bool CustomName { get; set; }

		public DateTime? Date { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

	}

}
