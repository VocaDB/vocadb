using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ReleaseEventForApiContract : IReleaseEvent, IEntryBase {

		bool IDeletableEntry.Deleted => false;
		string IEntryBase.DefaultName => Name;
		EntryType IEntryBase.EntryType => EntryType.ReleaseEvent;

		public ReleaseEventForApiContract() { }

		public ReleaseEventForApiContract(ReleaseEvent rel, ContentLanguagePreference languagePreference, ReleaseEventOptionalFields fields, IEntryThumbPersister thumbPersister) {

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

			if (rel.HasSeries) {
				SeriesId = rel.Series.Id;
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.AdditionalNames)) {
				AdditionalNames = rel.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Artists)) {
				Artists = rel.Artists.Select(a => new ArtistForEventContract(a, languagePreference)).ToArray();
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Description)) {
				Description = rel.Description;
			}

			if (thumbPersister != null && fields.HasFlag(ReleaseEventOptionalFields.MainPicture)) {
				MainPicture = EntryThumbForApiContract.Create(EntryThumb.Create(rel) ?? EntryThumb.Create(rel.Series), thumbPersister);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Names)) {
				Names = rel.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Series) && rel.HasSeries) {
				Series = new ReleaseEventSeriesContract(rel.Series, languagePreference);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.SongList) && rel.SongList != null) {
				SongList = new SongListBaseContract(rel.SongList);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.WebLinks)) {
				WebLinks = rel.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();
			}

		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set; }

		/// <summary>
		/// List of artist links.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public ArtistForEventContract[] Artists { get; set; }

		/// <summary>
		/// Event category. 
		/// This is NOT inherited from series at the moment (you need to check <see cref="Series"/> for the category).
		/// </summary>
		[DataMember]
		public EventCategory Category { get; set; }

		[DataMember]
		public DateTime? Date { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public DateTime? EndDate { get; set; }

		[DataMember]
		public int Id { get; set; }

		/// <summary>
		/// Main picture.
		/// This IS inherited from series.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ReleaseEventSeriesContract Series { get; set; }

		[DataMember]
		public int? SeriesId { get; set; }

		[DataMember]
		public int SeriesNumber { get; set; }

		[DataMember]
		public string SeriesSuffix { get; set; }

		[DataMember]
		public SongListBaseContract SongList { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public string UrlSlug { get; set; }

		[DataMember]
		public string VenueName { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public WebLinkForApiContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum ReleaseEventOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Artists = 2,
		Description = 4,
		MainPicture = 8,
		Names = 16,
		Series = 32,
		SongList = 64,
		WebLinks = 128

	}

}
