using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ReleaseEventWithArchivedVersionsContract : ReleaseEventContract
	{
		public ReleaseEventWithArchivedVersionsContract(ReleaseEvent ev, ContentLanguagePreference languagePreference)
			: base(ev, languagePreference)
		{
			ArchivedVersions = ev.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedEventVersionContract(a)).ToArray();
		}

		public ArchivedEventVersionContract[] ArchivedVersions { get; set; }
	}
}
