using System.Collections.Generic;
using System.Threading.Tasks;

namespace VocaDb.Model.Domain.Tags
{
	public interface ITagFactory
	{
		Task<Tag> CreateTagAsync(string englishName);
	}

	public static class ITagFactoryExtensions
	{
		// TODO: .NET Core async enumerable
		public static async Task<List<Tag>> CreateTagsAsync(this ITagFactory tagFactory, IEnumerable<string> englishNames)
		{
			var tags = new List<Tag>();
			foreach (var name in englishNames)
			{
				tags.Add(await tagFactory.CreateTagAsync(name));
			}
			return tags;
		}
	}
}
