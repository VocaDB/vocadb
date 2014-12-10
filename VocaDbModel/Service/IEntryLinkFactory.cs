using VocaDb.Model.Domain;

namespace VocaDb.Model.Service {

	/// <summary>
	/// Creates HTML anchors to common entry types.
	/// </summary>
	public interface IEntryLinkFactory {

		/// <summary>
		/// Creates HTML anchor tag for an entry.
		/// HTML will be encoded.
		/// </summary>
		/// <param name="entry">Entry reference. Cannot be null.</param>
		/// <returns>Enchor tag with a link for the entry. For example, &lt;a href='/S/3939'&gt;Miku!&lt;/a&gt;</returns>
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
