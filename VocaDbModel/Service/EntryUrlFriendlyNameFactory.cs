#nullable disable

using System.Collections.Concurrent;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service
{
	public class EntryUrlFriendlyNameFactory : IEntryUrlFriendlyNameFactory
	{
		private readonly ConcurrentDictionary<GlobalEntryId, string> _cachedNames = new();

#nullable enable
		public string GetUrlFriendlyName(IEntryWithNames entry)
		{
			ParamIs.NotNull(() => entry);

			return _cachedNames.GetOrAdd(new GlobalEntryId(entry.EntryType, entry.Id), _ => entry.Names.GetUrlFriendlyName());
		}
#nullable disable
	}

	public interface IEntryUrlFriendlyNameFactory
	{
		string GetUrlFriendlyName(IEntryWithNames entry);
	}
}
