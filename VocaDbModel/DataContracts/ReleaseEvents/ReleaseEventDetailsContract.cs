using System;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventDetailsContract : ReleaseEventContract {

		public ReleaseEventDetailsContract() {
			WebLinks = new WebLinkContract[0];
		}

		public ReleaseEventDetailsContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference) 
			: base(releaseEvent, true) {

			ParamIs.NotNull(() => releaseEvent);

			SeriesNumber = releaseEvent.SeriesNumber;
			SeriesSuffix = releaseEvent.SeriesSuffix;
			WebLinks = releaseEvent.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

			Albums = releaseEvent.Albums
				.Select(a => new AlbumContract(a, languagePreference))
				.OrderBy(a => a.Name)
				.ToArray();

			Songs = releaseEvent.Songs
				.Select(s => new SongForApiContract(s, languagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl))
				.OrderBy(s => s.Name)
				.ToArray();

		}

		public AlbumContract[] Albums { get; set; }

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		public int SeriesNumber { get; set; }

		public string SeriesSuffix { get; set; }

		public SongForApiContract[] Songs { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}
}
