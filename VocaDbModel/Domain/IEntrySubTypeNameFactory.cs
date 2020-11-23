using System.Globalization;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Domain
{

	public interface IEntrySubTypeNameFactory
	{

		string GetEntrySubTypeName(IEntryBase entryBase, IEnumTranslations enumTranslations, CultureInfo culture);

	}

}
