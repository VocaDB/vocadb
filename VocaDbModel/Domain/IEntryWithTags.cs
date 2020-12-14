#nullable disable

using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Domain
{
	public interface IEntryWithTags<TTagLink> : IEntryWithTags where TTagLink : TagUsage
	{
		new TagManager<TTagLink> Tags { get; }
	}
}
