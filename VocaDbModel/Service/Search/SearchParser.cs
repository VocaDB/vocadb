using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocaDb.Model.Service.Search {

	public static class SearchParser {

		private static SearchWord ParseWord(string word) {

			var propPos = word.IndexOf(':');

			if (propPos == -1)
				return new SearchWord(word);

			var propName = word.Substring(0, propPos);
			var val = word.Substring(propPos + 1).TrimStart();

			return new SearchWord(propName, val);

		}

		public static SearchWordCollection ParseQuery(string query) {

			var words = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(ParseWord).Distinct();

			return new SearchWordCollection(words);

		}

	}

}
