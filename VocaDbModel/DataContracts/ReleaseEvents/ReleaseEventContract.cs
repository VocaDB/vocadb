using System;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventContract : IReleaseEvent, IEntryImageInformation, IEntryBase, IEntryWithStatus {

		EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEvent;
		EntryType IEntryBase.EntryType => EntryType.ReleaseEvent;
		string IEntryImageInformation.Mime => PictureMime;
		string IEntryBase.DefaultName => Name;

		public ReleaseEventContract() {
			Description = string.Empty;
		}

		public ReleaseEventContract(ReleaseEvent ev, ContentLanguagePreference languagePreference, bool includeSeries = false, bool includeSeriesLinks = false)
			: this() {

			ParamIs.NotNull(() => ev);

			AdditionalNames = ev.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			Category = ev.Category;
			CustomName = ev.CustomName;
			Date = ev.Date;
			Deleted = ev.Deleted;
			Description = ev.Description;
			EndDate = ev.EndDate;
			Id = ev.Id;
			Name = ev.TranslatedName[languagePreference];
			PictureMime = ev.PictureMime;
			SongList = ObjectHelper.Convert(ev.SongList, s => new SongListBaseContract(s));
			Status = ev.Status;
			UrlSlug = ev.UrlSlug;
			Venue = ObjectHelper.Convert(ev.Venue, v => new VenueContract(v, languagePreference, includeSeriesLinks));
			VenueName = ev.VenueName;
			Version = ev.Version;

			if (includeSeries && ev.HasSeries) {
				Series = new ReleaseEventSeriesContract(ev.Series, languagePreference, includeSeriesLinks);
			}

		}

		public string AdditionalNames { get; set; }

		public EventCategory Category { get; set; }

		public bool CustomName { get; set; }

		public DateTime? Date { get; set; }

		public bool Deleted { get; set; }

		public string Description { get; set; }

		public DateTime? EndDate { get; set; }

		public bool HasVenueOrVenueName => Venue != null || !string.IsNullOrEmpty(VenueName);

		public int Id { get; set; }

		public EventCategory InheritedCategory => Series?.Category ?? Category;

		public string Name { get; set; }

		public string PictureMime { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

		public SongListBaseContract SongList { get; set; }

		public EntryStatus Status { get; set; }

		public string UrlSlug { get; set; }

		public VenueContract Venue { get; set; }

		public string VenueName { get; set; }

		public int Version { get; set; }

	}

}
