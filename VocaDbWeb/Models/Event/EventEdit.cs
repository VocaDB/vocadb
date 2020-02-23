using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Event {

	[PropertyModelBinder]
	public class EventEdit : IEntryImageInformation {

		EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEvent;
		string IEntryImageInformation.Mime => PictureMime;

		public EventEdit() {
			Description = SeriesSuffix = string.Empty;
		}

		public EventEdit(ReleaseEventSeriesContract seriesContract, IUserPermissionContext userContext)
			: this() {

			Series = seriesContract;

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(userContext).ToArray();

		}

		public EventEdit(ReleaseEventForEditContract contract, IUserPermissionContext userContext)
			: this() {

			ParamIs.NotNull(() => contract);

			Artists = contract.Artists;
			Category = contract.Category;
			CustomName = contract.CustomName = contract.CustomName;
			Date = contract.Date;
			DefaultNameLanguage = contract.DefaultNameLanguage;
			Description = contract.Description;
			EndDate = contract.EndDate;
			Id = contract.Id;
			Name = OldName = contract.Name;
			Names = contract.Names;
			PVs = contract.PVs;
			Series = contract.Series;
			SeriesNumber = contract.SeriesNumber;
			SeriesSuffix = contract.SeriesSuffix;
			SongList = contract.SongList;
			Status = contract.Status;
			Venue = contract.Venue;
			VenueName = contract.VenueName;
			WebLinks = contract.WebLinks;

			CopyNonEditableProperties(contract, userContext);

		}

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		[FromJson]
		public ArtistForEventContract[] Artists { get; set; }

		public EventCategory Category { get; set; }

		public bool CustomName { get; set; }

		[FromJson]
		public DateTime? Date { get; set; }

		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		public bool Deleted { get; set; }

		[StringLength(1000)]
		public string Description { get; set; }

		[FromJson]
		public DateTime? EndDate { get; set; }

		public int Id { get; set; }

		[StringLength(50)]
		public string Name { get; set; }

		[FromJson]
		public LocalizedStringWithIdContract[] Names { get; set; }

		public string OldName { get; set; }

		public string PictureMime { get; set; }

		[FromJson]
		public PVContract[] PVs { get; set; }

		[FromJson]
		public ReleaseEventSeriesContract Series { get; set; }

		[Display(Name = "Series suffix")]
		public string SeriesSuffix { get; set; }

		[Display(Name = "Series number")]
		public int SeriesNumber { get; set; }

		[FromJson]
		public SongListBaseContract SongList { get; set; }

		public EntryStatus Status { get; set; }

		public string UrlSlug { get; set; }

		[FromJson]
		public VenueContract Venue { get; set; }

		public string VenueName { get; set; }

		public int Version { get; set; }

		[FromJson]
		public WebLinkContract[] WebLinks { get; set; }

		public void CopyNonEditableProperties(ReleaseEventDetailsContract contract, IUserPermissionContext userContext) {

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(userContext).ToArray();

			if (contract != null) {
				Deleted = contract.Deleted;
				OldName = contract.Name;
				PictureMime = contract.PictureMime;
				UrlSlug = contract.UrlSlug;
				Version = contract.Version;
			}

		}

		public ReleaseEventForEditContract ToContract() {

			return new ReleaseEventForEditContract {
				Artists = Artists,
				Category = Category,
				CustomName = this.CustomName,
				Date = this.Date,
				DefaultNameLanguage = DefaultNameLanguage,
				Description = this.Description ?? string.Empty,
				EndDate = this.EndDate,
				Id = this.Id,
				Name = this.Name,
				Names = Names,
				PVs = PVs,
				Series = this.Series, 
				SeriesNumber = this.SeriesNumber,
				SeriesSuffix = this.SeriesSuffix ?? string.Empty,
				SongList = SongList,
				Status = Status,
				Venue = Venue,
				VenueName = VenueName,
				WebLinks = this.WebLinks
			};

		}

	}

}