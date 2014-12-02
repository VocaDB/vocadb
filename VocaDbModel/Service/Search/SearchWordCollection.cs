using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VocaDb.Model.Service.Search {

	public class SearchWordCollection : IEnumerable<SearchWord> {

		private readonly List<SearchWord> words;

		public SearchWordCollection(IEnumerable<SearchWord> words) {
			this.words = words.ToList();
		}

		public List<SearchWord> Words {
			get { return words; }
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public IEnumerator<SearchWord> GetEnumerator() {
			return Words.GetEnumerator();
		}

		public SearchWord[] TakeAll(string name) {
			var match = Words.Where(w => w.PropertyName != null && w.PropertyName.Equals(name, StringComparison.InvariantCultureIgnoreCase)).ToArray();
			Words.RemoveAll(match.Contains);
			return match;
		}

		public SearchWord TakeNext() {

			if (!Words.Any())
				return null;

			var w = Words.First();
			Words.Remove(w);

			return w;

		}

	}

}
