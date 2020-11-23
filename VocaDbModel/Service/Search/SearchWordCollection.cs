using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Service.Search
{

	public class SearchWordCollection : IEnumerable<SearchWord>
	{

		public SearchWordCollection(IEnumerable<SearchWord> words)
		{
			Words = words.ToList();
		}

		public List<SearchWord> Words { get; }

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<SearchWord> GetEnumerator()
		{
			return Words.GetEnumerator();
		}

		private IEnumerable<SearchWord> GetWords(string name)
		{
			return Words.Where(w => string.Equals(w.PropertyName, name, StringComparison.InvariantCultureIgnoreCase));
		}

		public SearchWord[] TakeAll(string name)
		{
			var match = GetWords(name).ToArray();
			Words.RemoveAll(match.Contains);
			return match;
		}

		public IEnumerable<string> GetValues(string name)
		{
			return GetWords(name).Select(w => w.Value);
		}

		public SearchWord TakeNext()
		{

			if (!Words.Any())
				return null;

			var w = Words.First();
			Words.Remove(w);

			return w;

		}

	}

}
