#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
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

namespace VocaDb.Web.Models.Event;

public class EventEdit : IEntryImageInformation
{
#nullable enable
	EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEvent;
	string? IEntryImageInformation.Mime => PictureMime;
	ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;
#nullable disable

	public EventEdit()
	{
		Description = SeriesSuffix = string.Empty;
	}

	public EventEdit(ReleaseEventSeriesContract seriesContract, VenueContract venueContract, IUserPermissionContext userContext)
		: this()
	{
		Series = seriesContract;
		Venue = venueContract;

		AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(userContext).ToArray();
	}

	public EventEdit(ReleaseEventForEditContract contract, IUserPermissionContext userContext)
		: this()
	{
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

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public ArtistForEventContract[] Artists { get; set; }

	public EventCategory Category { get; set; }

	public bool CustomName { get; set; }

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public DateTime? Date { get; set; }

	public ContentLanguageSelection DefaultNameLanguage { get; set; }

	public bool Deleted { get; set; }

	[StringLength(1000)]
	public string Description { get; set; }

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public DateTime? EndDate { get; set; }

	public int Id { get; set; }

	[StringLength(50)]
	public string Name { get; set; }

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public LocalizedStringWithIdContract[] Names { get; set; }

	public string OldName { get; set; }

#nullable enable
	public string? PictureMime { get; set; }
#nullable disable

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public PVContract[] PVs { get; set; }

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public ReleaseEventSeriesContract Series { get; set; }

	[Display(Name = "Series suffix")]
	public string SeriesSuffix { get; set; }

	[Display(Name = "Series number")]
	public int SeriesNumber { get; set; }

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public SongListBaseContract SongList { get; set; }

	public EntryStatus Status { get; set; }

	public string UrlSlug { get; set; }

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public VenueContract Venue { get; set; }

	public string VenueName { get; set; }

	public int Version { get; set; }

	[ModelBinder(BinderType = typeof(JsonModelBinder))]
	public WebLinkContract[] WebLinks { get; set; }

	public void CopyNonEditableProperties(ReleaseEventDetailsContract contract, IUserPermissionContext userContext)
	{
		AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(userContext).ToArray();

		if (contract != null)
		{
			Deleted = contract.Deleted;
			OldName = contract.Name;
			PictureMime = contract.PictureMime;
			UrlSlug = contract.UrlSlug;
			Version = contract.Version;
		}
	}

	public ReleaseEventForEditContract ToContract()
	{
		return new ReleaseEventForEditContract
		{
			Artists = Artists,
			Category = Category,
			CustomName = CustomName,
			Date = Date,
			DefaultNameLanguage = DefaultNameLanguage,
			Description = Description ?? string.Empty,
			EndDate = EndDate,
			Id = Id,
			Name = Name,
			Names = Names,
			PVs = PVs,
			Series = Series,
			SeriesNumber = SeriesNumber,
			SeriesSuffix = SeriesSuffix ?? string.Empty,
			SongList = SongList,
			Status = Status,
			Venue = Venue,
			VenueName = VenueName,
			WebLinks = WebLinks
		};
	}
}