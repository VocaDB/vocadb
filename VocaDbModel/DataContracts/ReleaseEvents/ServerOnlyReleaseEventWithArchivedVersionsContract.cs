#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ServerOnlyReleaseEventWithArchivedVersionsContract : ReleaseEventContract
	{
		public ServerOnlyReleaseEventWithArchivedVersionsContract(ReleaseEvent ev, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
			: base(ev, languagePreference)
		{
			ArchivedVersions = ev.ArchivedVersionsManager.Versions.Select(
				a => new ServerOnlyArchivedEventVersionContract(a, userIconFactory)).ToArray();
		}

		public ServerOnlyArchivedEventVersionContract[] ArchivedVersions { get; init; }
	}
}
