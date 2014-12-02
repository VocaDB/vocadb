using VocaDb.Model.Domain;
using VocaDb.Model.Service;

namespace VocaDb.Tests.TestSupport {

	public class FakeEntryLinkFactory : IEntryLinkFactory  {

		public string CreateEntryLink(IEntryBase entry) {
			return string.Empty;
		}

		public string CreateEntryLink(EntryType entryType, int id, string name) {
			return string.Empty;
		}

		public string GetFullEntryUrl(EntryType entryType, int id) {
			return string.Empty;
		}

	}

}
