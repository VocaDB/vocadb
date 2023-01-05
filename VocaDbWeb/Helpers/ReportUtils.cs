#nullable disable

using System.Collections;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Web.Helpers;

public static class ReportUtils
{
	public static IEnumerable GetReportTypes<TEnum>(TranslateableEnum<TEnum> translations, HashSet<TEnum> notesRequired)
		where TEnum : struct, Enum
	{
		return translations.Select(r => new
		{
			r.Id,
			r.Name,
			NotesRequired = notesRequired.Contains(r.Id)
		});
	}
}