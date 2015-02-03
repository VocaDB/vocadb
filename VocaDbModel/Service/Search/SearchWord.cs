using System;
using System.Linq;

namespace VocaDb.Model.Service.Search {

	public class SearchWord {

		public static SearchWord GetTerm(string query, params string[] testTerms) {

			return (
				from term in testTerms 
				where query.StartsWith(term + ":", StringComparison.InvariantCultureIgnoreCase) 
				select new SearchWord(term, query.Substring(term.Length + 1).TrimStart()))
			.FirstOrDefault();

		}

		private readonly string propertyName;
		private readonly string value;

		public SearchWord(string val) 
			: this(string.Empty, val) {}

		public SearchWord(string propName, string val) {
			propertyName = propName;
			value = val;
		}

		public string PropertyName {
			get { return propertyName; }
		}

		public string Value {
			get { return value; }
		}

		public bool Equals(SearchWord other) {

			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			return (PropertyName == other.PropertyName && Value == other.Value);

		}

		public override bool Equals(object obj) {
			return Equals(obj as SearchWord);
		}

		public override int GetHashCode() {
			unchecked {
				return ((propertyName != null ? propertyName.GetHashCode() : 0)*397) ^ (value != null ? value.GetHashCode() : 0);
			}
		}
	}

}
