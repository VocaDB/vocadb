using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Domain {

	public interface IEntryWithTags<TTagLink> where TTagLink : TagUsage {

		TagManager<TTagLink> Tags { get; }

	}

}
