using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts.UseCases
{

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForEditContract : ArtistContract
	{

		public ArtistForEditContract() { }

		public ArtistForEditContract(Artist artist, ContentLanguagePreference languagePreference, IAggregatedEntryImageUrlFactory imageStore)
			: base(artist, languagePreference)
		{

			BaseVoicebank = artist.BaseVoicebank != null ? new ArtistContract(artist.BaseVoicebank, languagePreference) : null;
			DefaultNameLanguage = artist.TranslatedName.DefaultLanguage;
			Description = new EnglishTranslatedStringContract(artist.Description);
			Groups = artist.Groups.Where(g => g.LinkType == ArtistLinkType.Group).Select(g => new ArtistForArtistContract(g, languagePreference)).OrderBy(g => g.Parent.Name).ToArray();
			Illustrator = artist.ArtistLinksOfType(ArtistLinkType.Illustrator, Domain.LinkDirection.ManyToOne).Select(g => new ArtistContract(g, languagePreference)).FirstOrDefault();
			Names = artist.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			Pictures = artist.Pictures.Select(p => new EntryPictureFileContract(p, imageStore)).ToList();
			UpdateNotes = string.Empty;
			VoiceProvider = artist.ArtistLinksOfType(ArtistLinkType.VoiceProvider, Domain.LinkDirection.ManyToOne).Select(g => new ArtistContract(g, languagePreference)).FirstOrDefault();
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

			AssociatedArtists = artist.Groups
				.Where(a => a.LinkType != ArtistLinkType.Group
					&& (a.Parent.Id != Illustrator?.Id || a.LinkType != ArtistLinkType.Illustrator)
					&& (a.Parent.Id != VoiceProvider?.Id || a.LinkType != ArtistLinkType.VoiceProvider))
				.Select(g => new ArtistForArtistContract(g, languagePreference))
				.ToArray();

		}

		[DataMember]
		public ArtistForArtistContract[] AssociatedArtists { get; set; }

		[DataMember]
		public ArtistContract BaseVoicebank { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		public EnglishTranslatedStringContract Description { get; set; }

		[DataMember]
		public ArtistForArtistContract[] Groups { get; set; }

		[DataMember]
		public ArtistContract Illustrator { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public IList<EntryPictureFileContract> Pictures { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

		[DataMember]
		public ArtistContract VoiceProvider { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
