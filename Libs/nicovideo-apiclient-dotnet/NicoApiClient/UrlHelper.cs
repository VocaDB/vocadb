using System.Text.RegularExpressions;

namespace VocaDb.NicoApi {

    internal static class UrlHelper {

        private static readonly Regex nicoImageRegex = new Regex(@"^http://tn(?:-skr\d)?\.smilevideo\.jp/smile\?i=([\d\.]+)$");

        public static string UpgradeToHttps(string url) {

            if (string.IsNullOrEmpty(url))
                return url;

            var nicoRegexMatch = nicoImageRegex.Match(url);

            if (nicoRegexMatch.Success) {
                return string.Format("https://tn.smilevideo.jp/smile?i=" + nicoRegexMatch.Groups[1]);
            }

            return url;

        }

    }

}
