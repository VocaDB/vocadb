using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract]
	public class TagForEditContract : TagContract {

		public TagForEditContract(Tag tag, IEnumerable<string> allCategoryNames, bool isEmpty)
			: base(tag) {

			AllCategoryNames = allCategoryNames.ToArray();
			IsEmpty = isEmpty;
			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);

		}

		[DataMember]
		public string[] AllCategoryNames { get; set; }

		[DataMember]
		public bool IsEmpty { get; set; }

		[DataMember]
		public EntryThumbContract Thumb { get; set; }

	}
}
