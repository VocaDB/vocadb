using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract]
	public class TagForEditContract : TagContract {

		public TagForEditContract(Tag tag, bool isEmpty)
			: base(tag) {

			IsEmpty = isEmpty;
			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);

		}

		[DataMember]
		public bool IsEmpty { get; set; }

		[DataMember]
		public EntryThumbContract Thumb { get; set; }

	}
}
