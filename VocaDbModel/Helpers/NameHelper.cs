using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts;

namespace VocaDb.Model.Helpers
{
	public static class NameHelper
	{
		public static string GetAdditionalNames(IEnumerable<string> names, string display)
		{
			return string.Join(", ", names.Where(n => n != display));
		}

		public static EntryNameContract GetName(INameManager nameManager, ContentLanguagePreference languagePreference)
		{
			var primary = nameManager.SortNames[languagePreference];

			return new EntryNameContract(primary, GetAdditionalNames(nameManager.AllValues, primary));
		}

		public static string[] MoveExactNamesToTop(string[] names, string term)
		{
			if (names == null || !names.Any())
				return names;

			var exactMatch = names.Where(n => n.StartsWith(term, StringComparison.InvariantCultureIgnoreCase)).ToArray();

			return exactMatch.Any() ? CollectionHelper.MoveToTop(names, exactMatch).ToArray() : names;
		}
	}
}
