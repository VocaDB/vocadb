namespace VocaDb.Model.Domain.Tags {

	public interface IEntryWithTags : IEntryBase {

		ITagManager Tags { get; }

	}

}
