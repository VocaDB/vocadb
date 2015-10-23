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

			IsEmpty = isEmpty;
			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);
			UpdateNotes = string.Empty;

		}

		[DataMember]
		public bool IsEmpty { get; set; }

		[DataMember]
		public EntryThumbContract Thumb { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

	}
}
