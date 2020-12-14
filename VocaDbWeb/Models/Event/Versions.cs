#nullable disable

using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Event
{
	public class Versions : Versions<ReleaseEventContract>
	{
		public Versions() { }

		public Versions(ReleaseEventWithArchivedVersionsContract contract, IEnumTranslations translator)
			: base(contract, contract.ArchivedVersions, translator) { }
	}
}