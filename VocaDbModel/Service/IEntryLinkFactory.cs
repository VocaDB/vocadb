using VocaDb.Model.Domain;

namespace VocaDb.Model.Service {

	/// <summary>
	/// Creates HTML anchors to common entry types.
	/// </summary>
	public interface IEntryLinkFactory {

		string CreateEntryLink(IEntryBase entry);

		string CreateEntryLink(EntryType entryType, int id, string name);

		string GetFullEntryUrl(EntryType entryType, int id);

	}

	public static class IEntryLinkFactoryExtender {
		
		public static string GetFullEntryUrl(this IEntryLinkFactory entryLinkFactory, IEntryBase entryBase) {
			return entryLinkFactory.GetFullEntryUrl(entryBase.EntryType, entryBase.Id);
		}

	}

}
