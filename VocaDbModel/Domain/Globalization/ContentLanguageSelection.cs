using System;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Domain.Globalization
{

	public enum ContentLanguageSelection
	{

		Unspecified = 0,

		Japanese = 1,

		Romaji = 2,

		English = 4,

	}

	[Flags]
	public enum ContentLanguageSelections
	{

		Unspecified = ContentLanguageSelection.Unspecified,
		Japanese = ContentLanguageSelection.Japanese,
		Romaji = ContentLanguageSelection.Romaji,
		English = ContentLanguageSelection.English,
		All = Unspecified | Japanese | Romaji | English

	}

	public static class ContentLanguageSelectionsExtender
	{

		public static IEnumerable<ContentLanguageSelection> ToIndividualSelections(this ContentLanguageSelections selections)
		{

			return EnumVal<ContentLanguageSelections>
				.GetIndividualValues(selections)
				.Where(s => s != ContentLanguageSelections.All)
				.Select(s => (ContentLanguageSelection)s);

		}

	}

}
