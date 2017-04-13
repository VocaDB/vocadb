using System;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventContract : IReleaseEvent, IEntryImageInformation, IEntryBase {

		EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEvent;
		EntryType IEntryBase.EntryType => EntryType.ReleaseEvent;
		string IEntryImageInformation.Mime => PictureMime;
		string IEntryBase.DefaultName => Name;

		public ReleaseEventContract() {
			Description = string.Empty;
		}

		public ReleaseEventContract(ReleaseEvent ev, bool includeSeries = false)
			: this() {

			ParamIs.NotNull(() => ev);

			Category = ev.Category;
			CustomName = ev.CustomName;
			Date = ev.Date;
			Deleted = ev.Deleted;
			Description = ev.Description;
			Id = ev.Id;
			Name = ev.Name;
			PictureMime = ev.PictureMime;
			SongList = ObjectHelper.Convert(ev.SongList, s => new SongListBaseContract(s));
			Status = ev.Status;
			UrlSlug = ev.UrlSlug;
			Venue = ev.Venue;
			Version = ev.Version;

			if (includeSeries && ev.Series != null)
				Series = new ReleaseEventSeriesContract(ev.Series);

		}

		public EventCategory Category { get; set; }

		public bool CustomName { get; set; }

		public DateTime? Date { get; set; }

		public bool Deleted { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public string PictureMime { get; set; }

		public ReleaseEventSeriesContract Series { get; set; }

		public SongListBaseContract SongList { get; set; }

		public EntryStatus Status { get; set; }

		public string UrlSlug { get; set; }

		public string Venue { get; set; }

		public int Version { get; set; }

	}

}
