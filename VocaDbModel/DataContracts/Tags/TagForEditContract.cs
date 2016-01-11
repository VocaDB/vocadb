using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract]
	public class TagForEditContract : TagContract {

		public TagForEditContract() {
			UpdateNotes = string.Empty;
		}

		public TagForEditContract(Tag tag, bool isEmpty, ContentLanguagePreference languagePreference)
			: base(tag, languagePreference) {

			DefaultNameLanguage = tag.TranslatedName.DefaultLanguage;
			IsEmpty = isEmpty;
			Names = tag.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);
			UpdateNotes = string.Empty;

		}

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[DataMember]
		public bool IsEmpty { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public EntryThumbContract Thumb { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

	}
}
