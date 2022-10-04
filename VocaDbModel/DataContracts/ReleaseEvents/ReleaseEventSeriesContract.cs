#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ReleaseEventSeriesContract : IEntryImageInformation, IEntryWithIntId, IEntryWithStatus
	{
#nullable enable
		string IEntryBase.DefaultName => Name;
#nullable disable
		EntryType IEntryBase.EntryType => EntryType.ReleaseEventSeries;
#nullable enable
		EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEventSeries;
		ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;
		string? IEntryImageInformation.Mime => PictureMime;
#nullable disable

		public ReleaseEventSeriesContract()
		{
			Description = string.Empty;
		}

#nullable enable
		public ReleaseEventSeriesContract(ReleaseEventSeries series, ContentLanguagePreference languagePreference, bool includeLinks = false)
			: this()
		{
			ParamIs.NotNull(() => series);

			AdditionalNames = series.Names.GetAdditionalNamesStringForLanguage(languagePreference);
			Category = series.Category;
			Deleted = series.Deleted;
			Description = series.Description;
			Id = series.Id;
			Name = series.TranslatedName[languagePreference];
			PictureMime = series.PictureMime;
			Status = series.Status;
			UrlSlug = series.UrlSlug;
			Version = series.Version;

			if (includeLinks)
			{
				WebLinks = series.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
			}
		}
#nullable disable

		[DataMember]
		public string AdditionalNames { get; init; }

		[DataMember]
		public EventCategory Category { get; init; }

		[DataMember]
		public bool Deleted { get; init; }

		[DataMember]
		public string Description { get; init; }

		[DataMember]
		public int Id { get; set; }

#nullable enable
		[DataMember]
		public string Name { get; init; }

		[DataMember]
		public string? PictureMime { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public string UrlSlug { get; init; }
#nullable disable

		[DataMember]
		public int Version { get; init; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; init; }

#nullable enable
		public override string ToString()
		{
			return $"release event series {Name} [{Id}]";
		}
#nullable disable
	}
}
