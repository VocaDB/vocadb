using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventWithArchivedVersionsContract : ReleaseEventContract {

		public ReleaseEventWithArchivedVersionsContract(ReleaseEvent ev)
			: base(ev) {

			ArchivedVersions = ev.ArchivedVersionsManager.Versions.Select(
				a => new ArchivedEventVersionContract(a)).ToArray();

		}

		public ArchivedEventVersionContract[] ArchivedVersions { get; set; }

	}
}
