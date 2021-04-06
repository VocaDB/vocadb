#nullable disable

using VocaDb.Model.DataContracts;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Models.Shared
{
	public class ViewVersion<TEntry>
	{
		public ViewVersion(TEntry entry, IEnumTranslations enumTranslations, int comparedVersionId)
		{
			Entry = entry;
			EnumTranslations = enumTranslations;
			ComparedVersionId = comparedVersionId;
		}

		public int ComparedVersionId { get; }

		public IEnumTranslations EnumTranslations { get; }

		public TEntry Entry { get; set; }

		public ArchivedObjectVersion Version(ServerOnlyArchivedObjectVersionContract contract)
		{
			return contract != null ? ArchivedObjectVersion.Create(contract, EnumTranslations) : null;
		}
	}
}