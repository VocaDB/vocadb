using System;
using System.Text;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.BBCode {

	public class AutoLinkTransformer : IBBCodeElementTransformer {

		private static readonly Regex regex = new Regex(@"http[s]?\:[a-zA-Z0-9_\#\-\.\:\/\%\?\&\=\+\(\)]+");

		public static string GetLink(Match match) {

			var text = match.Value;
			Uri uri;

			if (Uri.TryCreate(text, UriKind.Absolute, out uri)) {
				return string.Format("<a href='{0}'>{0}</a>", text);
			} else {
				return text;
			}


		}

		public void ApplyTransform(StringBuilder bbCode) {
			
			BBCodeConverter.RegexReplace(bbCode, regex, GetLink);

		}

	}

}
