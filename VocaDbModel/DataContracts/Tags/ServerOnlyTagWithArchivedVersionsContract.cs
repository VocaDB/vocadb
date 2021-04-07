#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	public class ServerOnlyTagWithArchivedVersionsContract : TagContract
	{
		public ServerOnlyTagWithArchivedVersionsContract(Tag tag, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
			: base(tag, languagePreference)
		{
			ArchivedVersions = tag.ArchivedVersionsManager.Versions.Select(
				a => new ServerOnlyArchivedTagVersionContract(a, userIconFactory)).ToArray();
		}

		public ServerOnlyArchivedTagVersionContract[] ArchivedVersions { get; init; }
	}
}
