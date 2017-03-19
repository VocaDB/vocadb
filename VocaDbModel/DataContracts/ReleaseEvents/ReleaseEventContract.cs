using System;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventContract : IReleaseEvent {

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
			SongList = ObjectHelper.Convert(ev.SongList, s => new SongListBaseContract(s));
			UrlSlug = ev.UrlSlug;

			if (includeSeries && ev.Series != null)
				Series = new ReleaseEventSeriesContract(ev.Series);

		}

		public bool CustomName { get; set; }

		public DateTime? Date { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

		public SongListBaseContract SongList { get; set; }

		public string UrlSlug { get; set; }

	}

}
