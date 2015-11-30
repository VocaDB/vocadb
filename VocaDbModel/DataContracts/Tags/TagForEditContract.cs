using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract]
	public class TagForEditContract : TagContract {

		public TagForEditContract() {
			UpdateNotes = string.Empty;
		}

		public TagForEditContract(Tag tag, bool isEmpty)
			: base(tag) {

			EnglishName = tag.EnglishName;
			IsEmpty = isEmpty;
			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);
			UpdateNotes = string.Empty;

		}

		[DataMember]
		public string EnglishName { get; set; }

		[DataMember]
		public bool IsEmpty { get; set; }

		[DataMember]
		public EntryThumbContract Thumb { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

	}
}
