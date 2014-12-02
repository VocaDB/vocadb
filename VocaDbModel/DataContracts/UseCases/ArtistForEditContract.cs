using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForEditContract : ArtistContract {

		public ArtistForEditContract() { }

		public ArtistForEditContract(Artist artist, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			BaseVoicebank = artist.BaseVoicebank != null ? new ArtistContract(artist.BaseVoicebank, languagePreference) : null;
			DefaultNameLanguage = artist.TranslatedName.DefaultLanguage;
			Description = artist.Description;
			Groups = artist.Groups.Select(g => new GroupForArtistContract(g, languagePreference)).OrderBy(g => g.Group.Name).ToArray();
			Names = artist.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			Pictures = artist.Pictures.Select(p => new EntryPictureFileContract(p)).ToList();
			UpdateNotes = string.Empty;
			WebLinks = artist.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

		}

		[DataMember]
		public ArtistContract BaseVoicebank { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public GroupForArtistContract[] Groups { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public IList<EntryPictureFileContract> Pictures { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
