using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NHibernate.Linq.Functions;

namespace VocaDb.Model.Service.VideoServices {

	public class RegexLinkMatcher {

		private readonly string baseUrl;
		private readonly Regex regex;

		public RegexLinkMatcher(string baseUrl, string regexStr) {

			regex = new Regex(regexStr, RegexOptions.IgnoreCase);
			this.baseUrl = baseUrl;

		}

		public string GetId(string url) {

			var match = regex.Match(url);

			if (match.Groups.Count < 2)
				throw new InvalidOperationException("Regex needs a group for the ID");

			var group = match.Groups[1];
			return group.Value;

		}

		public bool IsMatch(string url) {

			return regex.IsMatch(url);

		}

		public string MakeLinkFromUrl(string url) {

			return MakeLinkFromId(GetId(url));

		}

		public string MakeLinkFromId(string id) {

			return string.Format(baseUrl, id);

		}

	}

}
