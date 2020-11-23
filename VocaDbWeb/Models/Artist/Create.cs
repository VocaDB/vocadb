using System.Linq;
using System.ComponentModel.DataAnnotations;
using ViewRes;
using ViewRes.Artist;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Helpers;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Web.Models.Artist
{

	public class Create
	{

		public Create()
		{
			ArtistType = ArtistType.Producer;
			Description = string.Empty;
			WebLinkCategory = WebLinkCategory.Other;
			WebLinkDescription = string.Empty;
		}

		[Display(ResourceType = typeof(CreateStrings), Name = "Description")]
		[StringLength(4000)]
		public string Description { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "ArtistType")]
		public ArtistType ArtistType { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "Draft")]
		public bool Draft { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "EnglishName")]
		[StringLength(255)]
		public string NameEnglish { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "NonEnglishName")]
		[StringLength(255)]
		public string NameOriginal { get; set; }

		[Display(ResourceType = typeof(EntryCreateStrings), Name = "RomajiName")]
		[StringLength(255)]
		public string NameRomaji { get; set; }

		public WebLinkCategory WebLinkCategory { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "WebLinkDescription")]
		[StringLength(512)]
		public string WebLinkDescription { get; set; }

		[Display(ResourceType = typeof(CreateStrings), Name = "WebLinkURL")]
		[StringLength(512)]
		public string WebLinkUrl { get; set; }

		public CreateArtistContract ToContract()
		{

			return new CreateArtistContract
			{
				ArtistType = this.ArtistType,
				Description = this.Description ?? string.Empty,
				Draft = this.Draft,
				Names = LocalizedStringHelper.SkipNullAndEmpty(NameOriginal, NameRomaji, NameEnglish).ToArray(),
				WebLink = (!string.IsNullOrWhiteSpace(WebLinkUrl) ? new WebLinkContract
				{
					Description = WebLinkDescription ?? string.Empty,
					Url = WebLinkUrl,
					Category = WebLinkCategory
				} : null)
			};

		}

	}
}