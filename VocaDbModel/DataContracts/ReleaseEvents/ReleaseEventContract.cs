#nullable disable

using System;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ReleaseEventContract : IReleaseEvent, IEntryImageInformation, IEntryBase, IEntryWithStatus
	{
		EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEvent;
		EntryType IEntryBase.EntryType => EntryType.ReleaseEvent;
		string IEntryImageInformation.Mime => PictureMime;
		ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;
		string IEntryBase.DefaultName => Name;

		public ReleaseEventContract()
		{
			Description = string.Empty;
		}

#nullable enable
		public ReleaseEventContract(ReleaseEvent ev, ContentLanguagePreference languagePreference, bool includeSeries = false, bool includeSeriesLinks = false)
			: this()
		{
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

			if (includeSeries && ev.HasSeries)
			{
				Series = new ReleaseEventSeriesContract(ev.Series, languagePreference, includeSeriesLinks);
			}
		}
#nullable disable

		public string AdditionalNames { get; init; }

		public EventCategory Category { get; init; }

		public bool CustomName { get; set; }

		public DateTime? Date { get; init; }

		public bool Deleted { get; init; }

		public string Description { get; init; }

		public DateTime? EndDate { get; init; }

		public bool HasVenueOrVenueName => Venue != null || !string.IsNullOrEmpty(VenueName);

		public int Id { get; set; }

		public EventCategory InheritedCategory => Series?.Category ?? Category;

		public string Name { get; init; }

		public string PictureMime { get; init; }

		public ReleaseEventSeriesContract Series { get; set; }

		public SongListBaseContract SongList { get; init; }

		public EntryStatus Status { get; init; }

		public string UrlSlug { get; init; }

		public VenueContract Venue { get; init; }

		public string VenueName { get; init; }

		public int Version { get; init; }
	}
}
