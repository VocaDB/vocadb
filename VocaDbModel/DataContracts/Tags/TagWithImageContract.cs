using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagWithImageContract : TagContract {

		public TagWithImageContract() { }

		public TagWithImageContract(Tag tag)
			: base(tag) {
			
			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);

		}

		public EntryThumbContract Thumb { get; set; }

	}
}
