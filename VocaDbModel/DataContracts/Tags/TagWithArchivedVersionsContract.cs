#nullable disable

using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	[Obsolete]
	public class TagWithArchivedVersionsContract : TagContract
	{
		public TagWithArchivedVersionsContract(Tag tag, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
			: base(tag, languagePreference)
		{
			ArchivedVersions = tag.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedTagVersionContract(a, userIconFactory)).ToArray();
		}

		public ArchivedTagVersionContract[] ArchivedVersions { get; init; }
	}
}
