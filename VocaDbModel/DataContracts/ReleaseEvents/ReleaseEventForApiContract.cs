#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ReleaseEventForApiContract : IReleaseEvent, IEntryBase
	{
		bool IDeletableEntry.Deleted => false;
		string IEntryBase.DefaultName => Name;
		EntryType IEntryBase.EntryType => EntryType.ReleaseEvent;

		public ReleaseEventForApiContract() { }

		public ReleaseEventForApiContract(ReleaseEvent rel, ContentLanguagePreference languagePreference, ReleaseEventOptionalFields fields, IAggregatedEntryImageUrlFactory thumbPersister)
		{
			Category = rel.Category;
			Date = rel.Date;
			EndDate = rel.EndDate;
			Id = rel.Id;
			Name = rel.TranslatedName[languagePreference];
			SeriesNumber = rel.SeriesNumber;
			SeriesSuffix = rel.SeriesSuffix;
			Status = rel.Status;
			UrlSlug = rel.UrlSlug;
			VenueName = rel.VenueName;
			Version = rel.Version;

			if (rel.HasSeries)
			{
				SeriesId = rel.Series.Id;
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.AdditionalNames))
			{
				AdditionalNames = rel.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Artists))
			{
				Artists = rel.Artists.Select(a => new ArtistForEventContract(a, languagePreference)).ToArray();
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Description))
			{
				Description = rel.Description;
			}

			if (thumbPersister != null && fields.HasFlag(ReleaseEventOptionalFields.MainPicture))
			{
				MainPicture = EntryThumbForApiContract.Create(EntryThumb.Create(rel) ?? EntryThumb.Create(rel.Series), thumbPersister);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Names))
			{
				Names = rel.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Series) && rel.HasSeries)
			{
				Series = new ReleaseEventSeriesContract(rel.Series, languagePreference);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.SongList) && rel.SongList != null)
			{
				SongList = new SongListBaseContract(rel.SongList);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Tags))
			{
				Tags = rel.Tags.ActiveUsages.Select(t => new TagUsageForApiContract(t, languagePreference)).ToArray();
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Venue) && rel.Venue != null)
			{
				Venue = new VenueForApiContract(rel.Venue, languagePreference, VenueOptionalFields.None);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.WebLinks))
			{
				WebLinks = rel.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();
			}
		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; init; }

		/// <summary>
		/// List of artist links.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ArtistForEventContract[] Artists { get; init; }

		/// <summary>
		/// Event category. 
		/// This is NOT inherited from series at the moment (you need to check <see cref="Series"/> for the category).
		/// </summary>
		[DataMember]
		public EventCategory Category { get; init; }

		[DataMember]
		public DateTime? Date { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; init; }

		[DataMember]
		public DateTime? EndDate { get; init; }

		public bool HasVenueOrVenueName => Venue != null || !string.IsNullOrEmpty(VenueName);

		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Main picture.
		/// This IS inherited from series.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; init; }

		[DataMember]
		public string Name { get; init; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public ReleaseEventSeriesContract Series { get; init; }

		[DataMember]
		public int? SeriesId { get; init; }

		[DataMember]
		public int SeriesNumber { get; init; }

		[DataMember]
		public string SeriesSuffix { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public SongListBaseContract SongList { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public TagUsageForApiContract[] Tags { get; init; }

		[DataMember]
		public string UrlSlug { get; init; }

		[DataMember]
		public VenueForApiContract Venue { get; init; }

		[DataMember]
		public string VenueName { get; init; }

		[DataMember]
		public int Version { get; init; }

		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; init; }
	}

	[Flags]
	public enum ReleaseEventOptionalFields
	{
		None = 0,
		AdditionalNames = 1 << 0,
		Artists = 1 << 1,
		Description = 1 << 2,
		MainPicture = 1 << 3,
		Names = 1 << 4,
		Series = 1 << 5,
		SongList = 1 << 6,
		Tags = 1 << 7,
		Venue = 1 << 8,
		WebLinks = 1 << 9,
	}
}
