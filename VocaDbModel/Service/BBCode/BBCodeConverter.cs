using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace VocaDb.Model.Service.BBCode {

	/// <summary>
	/// Converts BBCode into HTML
	/// </summary>
	public class BBCodeConverter {

		private readonly IBBCodeElementTransformer[] transformers;

		/// <summary>
		/// Replaces instances of a regex in a string.
		/// The match may occur multiple times, and all instances will be replaced.
		/// </summary>
		/// <param name="bbCode">Source string to be processed. Cannot be null.</param>
		/// <param name="regex">Regex to be matched. Cannot be null.</param>
		/// <param name="replacementFunc">Replacement operation to be performed for the matches. Cannot be null.</param>
		public static void RegexReplace(StringBuilder bbCode, Regex regex, Func<Match, string> replacementFunc) {

			var matches = regex.Matches(bbCode.ToString());

			var indexOffset = 0;

			foreach (Match match in matches) {

				var result = replacementFunc(match);

				if (result != match.Value) {
					bbCode.Replace(match.Value, result, match.Index + indexOffset, match.Length);
					indexOffset += (result.Length - match.Value.Length);
				}

			}

		}

		/// <summary>
		/// Initializes converter with a list of transformations to be applied.
		/// </summary>
		/// <param name="transformers">Transformations. Cannot be null.</param>
		public BBCodeConverter(IEnumerable<IBBCodeElementTransformer> transformers) {

			ParamIs.NotNull(() => transformers);

			this.transformers = transformers.ToArray();

		}

		/// <summary>
		/// Converts source text into HTML, applying transformations.
		/// </summary>
		/// <param name="source">Source text, which must not be HTML-encoded. All HTML contained in the source will be encoded. Can be null or empty.</param>
		/// <returns>Encoded and transformed HTML. Can be null or empty, if the source was null or empty.</returns>
		/// <remarks>
		/// Since the result is already HTML-encoded, it must be printed using Html.Raw, so that it won't be encoded again.
		/// The input 
		/// </remarks>
		public string ConvertToHtml(string source) {

			if (string.IsNullOrEmpty(source))
				return source;

			source = HttpUtility.HtmlEncode(source);
			var replaced = new StringBuilder(source);

			foreach (var transformer in transformers)
				transformer.ApplyTransform(replaced);

			return replaced.ToString();

		}
	}

}
