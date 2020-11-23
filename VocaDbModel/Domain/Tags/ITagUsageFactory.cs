
namespace VocaDb.Model.Domain.Tags
{
	public interface ITagUsageFactory<T> where T : TagUsage
	{
		T CreateTagUsage(Tag tag);

		T CreateTagUsage(Tag tag, T usage);
	}
}
