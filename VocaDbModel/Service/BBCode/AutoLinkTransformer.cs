using System;
using System.Text;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.BBCode {

	public class AutoLinkTransformer : IBBCodeElementTransformer {

		// Markdown-style link or plain link
		private static readonly Regex regex = new Regex(@"(?:\[([\w ]+)\]\((http[s]?\:[a-zA-Z0-9_\#\-\.\:\/\%\?\&\=\+\(\)]+)\))|(http[s]?\:[a-zA-Z0-9_\#\-\.\:\/\%\?\&\=\+\(\)]+)");

		public static string GetLink(Match match) {

			var text = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Value;
			var uriText = match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value;
			Uri uri;

			if (Uri.TryCreate(uriText, UriKind.Absolute, out uri)) {
				return string.Format("<a href='{0}'>{1}</a>", uriText, text);
			} else {
				return match.Value;
			}


		}

		public void ApplyTransform(StringBuilder bbCode) {
			
			BBCodeConverter.RegexReplace(bbCode, regex, GetLink);

		}

	}

}
