namespace VocaDb.Model.Domain.Tags {

	public class TagMergeRecord : MergeRecord<Tag> {

		public TagMergeRecord() { }

		public TagMergeRecord(Tag source, Tag target)
			: base(source, target) { }

	}

}
