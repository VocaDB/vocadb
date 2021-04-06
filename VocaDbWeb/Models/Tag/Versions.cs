#nullable disable

using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Models.Shared;

namespace VocaDb.Web.Models.Tag
{
	public class Versions : Versions<TagContract>
	{
		public Versions() { }

		public Versions(ServerOnlyTagWithArchivedVersionsContract contract, IEnumTranslations translator)
			: base(contract, contract.ArchivedVersions, translator) { }
	}
}