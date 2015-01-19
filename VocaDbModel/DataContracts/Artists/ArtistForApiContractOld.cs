using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.Artists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArtistForApiContractOld : ArtistContract {

		public ArtistForApiContractOld() { }

		public ArtistForApiContractOld(Artist artist, ArtistMergeRecord mergeRecord, ContentLanguagePreference languagePreference)
			: base(artist, languagePreference) {

			CreateDate = artist.CreateDate;
			Description = artist.Description[languagePreference];
			Groups = artist.Groups.Select(g => new ArtistContract(g.Group, languagePreference)).ToArray();
			Members = artist.Members.Select(m => new ArtistContract(m.Member, languagePreference)).ToArray();
			Tags = artist.Tags.Usages.Select(u => new TagUsageContract(u)).ToArray();
			WebLinks = artist.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();

			if (mergeRecord != null)
				MergedTo = mergeRecord.Target.Id;

		}

		public ArtistForApiContractOld(Artist artist, ContentLanguagePreference languagePreference, ArtistEditableFields includedFields)
			: base(artist, languagePreference) {

			CreateDate = artist.CreateDate;
			Description = artist.Description[languagePreference];

			if (includedFields.HasFlag(ArtistEditableFields.Groups)) {
				Groups = artist.Groups.Select(g => new ArtistContract(g.Group, languagePreference)).ToArray();
				Members = artist.Members.Select(m => new ArtistContract(m.Member, languagePreference)).ToArray();
			}

			Tags = artist.Tags.Usages.Select(u => new TagUsageContract(u)).ToArray();

			if (includedFields.HasFlag(ArtistEditableFields.WebLinks))
				WebLinks = artist.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();

		}

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArtistContract[] Groups { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArtistContract[] Members { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int MergedTo { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public TagUsageContract[] Tags { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
