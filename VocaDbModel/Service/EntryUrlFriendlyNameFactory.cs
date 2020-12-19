#nullable disable

using System.Collections.Concurrent;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service
{
	public class EntryUrlFriendlyNameFactory : IEntryUrlFriendlyNameFactory
	{
		private readonly ConcurrentDictionary<GlobalEntryId, string> _cachedNames = new();

		public string GetUrlFriendlyName(IEntryWithNames entry)
		{
			ParamIs.NotNull(() => entry);

			return _cachedNames.GetOrAdd(new GlobalEntryId(entry.EntryType, entry.Id), _ => entry.Names.GetUrlFriendlyName());
		}
	}

	public interface IEntryUrlFriendlyNameFactory
	{
		string GetUrlFriendlyName(IEntryWithNames entry);
	}
}
