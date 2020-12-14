#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Service;

namespace VocaDb.Tests.TestSupport
{
	public class FakeEntryLinkFactory : IEntryLinkFactory
	{
		public string CreateEntryLink(IEntryBase entry, string slug)
		{
			return entry?.ToString();
		}

		public string CreateEntryLink(EntryType entryType, int id, string name, string slug)
		{
			return string.Empty;
		}

		public string GetFullEntryUrl(EntryType entryType, int id, string slug)
		{
			return string.Empty;
		}
	}
}
