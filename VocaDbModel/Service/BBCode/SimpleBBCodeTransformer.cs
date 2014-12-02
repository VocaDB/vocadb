using System;
using System.Text;
using System.Text.RegularExpressions;

namespace VocaDb.Model.Service.BBCode {

	/// <summary>
	/// Replaces a BBCode tag with a simple HTML tag. Doesn't support attributes.
	/// For example: [b]Bold text[/b] -> &lt;b&gt;Bold text&lt;/b&gt;
	/// </summary>
	public class SimpleBBCodeTransformer : IBBCodeElementTransformer {

		public static void ReplaceSimpleTag(StringBuilder bbCode, string bbTagName, string htmlTagName) {

			BBCodeConverter.RegexReplace(bbCode, new Regex(String.Format(@"\[{0}\]([^\]]+)\[\/{0}\]", bbTagName)), 
				match => String.Format("<{0}>{1}</{0}>", htmlTagName, match));

		}

		private readonly string bbTagName;
		private readonly string htmlTagName;

		public SimpleBBCodeTransformer(string bbTagName, string htmlTagName) {
			this.bbTagName = bbTagName;
			this.htmlTagName = htmlTagName;
		}

		public void ApplyTransform(StringBuilder bbCode) {

			ReplaceSimpleTag(bbCode, bbTagName, htmlTagName);

		}

	}
}
