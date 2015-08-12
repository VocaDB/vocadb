using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Service.TagFormatting {

	public abstract class SongCsvFileFormatter<T> {

		protected abstract string GetFieldValue(string fieldName, T track, ContentLanguagePreference languagePreference);

		private void ReplaceField(
			string tokenName, string tokenStr, StringBuilder sb, T track, ContentLanguagePreference languagePreference) {

			var val = GetField(GetFieldValue(tokenName.ToLowerInvariant(), track, languagePreference));
			sb.Replace(tokenStr, val);

		}

		private string ApplyFormat(T track, string format, ContentLanguagePreference languagePreference, MatchCollection fieldMatches) {

			var sb = new StringBuilder(format);

			foreach (Match match in fieldMatches) {
				ReplaceField(match.Groups[1].Value, match.Value, sb, track, languagePreference);
			}

			return sb.ToString();

		}

		private static string GetField(string val) {

			if (string.IsNullOrEmpty(val))
				return string.Empty;

			if (!val.Contains(";"))
				return val;
			else
				return string.Format("\"{0}\"", val);

		}

		protected string ApplyFormat(IEnumerable<T> songs, string format, ContentLanguagePreference languagePreference, bool includeHeader) {

			var sb = new StringBuilder();

			var fieldRegex = new Regex(@"%([\w\.!]+)%");
			var formatFields = fieldRegex.Matches(format);

			if (includeHeader) {
				sb.AppendLine(string.Join(";", formatFields.Cast<Match>().Select(f => GetField(f.Groups[1].Value))));
			}

			foreach (var song in songs)
				sb.AppendLine(ApplyFormat(song, format, languagePreference, formatFields));

			return sb.ToString();

		}

	}
}
