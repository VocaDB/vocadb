#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ServerOnlyReleaseEventWithArchivedVersionsContract : ReleaseEventContract
	{
		public ServerOnlyReleaseEventWithArchivedVersionsContract(ReleaseEvent ev, ContentLanguagePreference languagePreference)
			: base(ev, languagePreference)
		{
			ArchivedVersions = ev.ArchivedVersionsManager.Versions.Select(
				a => new ServerOnlyArchivedEventVersionContract(a)).ToArray();
		}

		public ServerOnlyArchivedEventVersionContract[] ArchivedVersions { get; init; }
	}
}
