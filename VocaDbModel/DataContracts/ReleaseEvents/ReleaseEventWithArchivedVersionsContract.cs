#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	public class ReleaseEventWithArchivedVersionsContract : ReleaseEventContract
	{
		public ReleaseEventWithArchivedVersionsContract(ReleaseEvent ev, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory)
			: base(ev, languagePreference)
		{
			ArchivedVersions = ev.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedEventVersionContract(a, userIconFactory)).ToArray();
		}

		public ArchivedEventVersionContract[] ArchivedVersions { get; init; }
	}
}
