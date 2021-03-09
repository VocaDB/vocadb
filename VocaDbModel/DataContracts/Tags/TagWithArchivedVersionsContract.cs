#nullable disable

using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class TagWithArchivedVersionsContract : TagContract
	{
		public TagWithArchivedVersionsContract(Tag tag, ContentLanguagePreference languagePreference)
			: base(tag, languagePreference)
		{
			ArchivedVersions = tag.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedTagVersionContract(a)).ToArray();
		}

		public ArchivedTagVersionContract[] ArchivedVersions { get; init; }
	}
}
