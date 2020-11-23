using System.Linq;
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

		string IEntryBase.DefaultName => Name;
		EntryType IEntryBase.EntryType => EntryType.ReleaseEventSeries;
		EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEventSeries;
		ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;
		string IEntryImageInformation.Mime => PictureMime;

		public ReleaseEventSeriesContract()
		{
			Description = string.Empty;
		}

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

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public EventCategory Category { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PictureMime { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public string UrlSlug { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

		public override string ToString()
		{
			return string.Format("release event series {0} [{1}]", Name, Id);
		}

	}

}
