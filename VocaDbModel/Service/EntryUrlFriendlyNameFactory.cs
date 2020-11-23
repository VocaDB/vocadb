using System.Collections.Concurrent;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Service
{

	public class EntryUrlFriendlyNameFactory : IEntryUrlFriendlyNameFactory
	{

		private readonly ConcurrentDictionary<GlobalEntryId, string> cachedNames = new ConcurrentDictionary<GlobalEntryId, string>();

		public string GetUrlFriendlyName(IEntryWithNames entry)
		{

			ParamIs.NotNull(() => entry);

			return cachedNames.GetOrAdd(new GlobalEntryId(entry.EntryType, entry.Id), _ => entry.Names.GetUrlFriendlyName());

		}

	}

	public interface IEntryUrlFriendlyNameFactory
	{

		string GetUrlFriendlyName(IEntryWithNames entry);

	}

}
