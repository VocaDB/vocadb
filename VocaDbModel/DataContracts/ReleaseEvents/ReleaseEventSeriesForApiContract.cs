using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventSeriesForApiContract {

		public ReleaseEventSeriesForApiContract() { }

		public ReleaseEventSeriesForApiContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference, ReleaseEventSeriesOptionalFields fields, IAggregatedEntryImageUrlFactory thumbPersister) {

			Category = series.Category;
			Id = series.Id;
			Name = series.TranslatedName[languagePreference];
			Status = series.Status;
			UrlSlug = series.UrlSlug;
			Version = series.Version;

			if (fields.HasFlag(ReleaseEventSeriesOptionalFields.AdditionalNames)) {
				AdditionalNames = series.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			}

			if (fields.HasFlag(ReleaseEventSeriesOptionalFields.Description)) {
				Description = series.Description;
			}

			if (fields.HasFlag(ReleaseEventSeriesOptionalFields.Events)) {
				Events = series.Events.Select(e => new ReleaseEventForApiContract(e, languagePreference, ReleaseEventOptionalFields.None, thumbPersister)).ToArray();
			}

			if (thumbPersister != null && fields.HasFlag(ReleaseEventSeriesOptionalFields.MainPicture)) {
				MainPicture = EntryThumbForApiContract.Create(EntryThumb.Create(series), thumbPersister);
			}

			if (fields.HasFlag(ReleaseEventSeriesOptionalFields.Names)) {
				Names = series.Names.Select(n => new LocalizedStringContract(n)).ToArray();
			}

			if (fields.HasFlag(ReleaseEventSeriesOptionalFields.WebLinks)) {
				WebLinks = series.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();
			}

		}

		/// <summary>
		/// Comma-separated list of all other names that aren't the display name.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public string AdditionalNames { get; set; }

		[DataMember]
		public EventCategory Category { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ReleaseEventForApiContract[] Events { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// List of all names for this entry. Optional field.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public string UrlSlug { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public WebLinkForApiContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum ReleaseEventSeriesOptionalFields {

		None = 0,
		AdditionalNames = 1,
		Description = 2,
		Events = 4,
		MainPicture = 8,
		Names = 16,
		WebLinks = 32

	}

}
