#nullable disable

using System;
using System.Linq;

namespace VocaDb.Model.Service.Search
{
	public class SearchWord
	{
		public static SearchWord GetTerm(string query, params string[] testTerms)
		{
			return (
				from term in testTerms
				where query.StartsWith(term + ":", StringComparison.InvariantCultureIgnoreCase)
				select new SearchWord(term, query.Substring(term.Length + 1).TrimStart()))
			.FirstOrDefault();
		}

		public SearchWord(string val)
			: this(string.Empty, val) { }

		public SearchWord(string propName, string val)
		{
			PropertyName = propName;
			Value = val;
		}

		public string PropertyName { get; }

		public string Value { get; }

#nullable enable
		public bool Equals(SearchWord? other)
		{
			if (other == null)
				return false;

			if (ReferenceEquals(this, other))
				return true;

			return (PropertyName == other.PropertyName && Value == other.Value);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as SearchWord);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((PropertyName != null ? PropertyName.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}
#nullable disable

		public override string ToString()
		{
			return !string.IsNullOrEmpty(PropertyName) ? PropertyName + ":" + Value : Value;
		}
	}
}
