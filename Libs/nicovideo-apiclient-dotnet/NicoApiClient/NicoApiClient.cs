using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace VocaDb.NicoApi {

    /// <summary>
    /// Various methods for the NicoNicoDouga video API.
    /// </summary>
    public class NicoApiClient {

        /// <summary>
        /// Initializes Nico API client.
        /// </summary>
        /// <param name="httpClient">HTTP client. Can be null, in which case new client is constructed. Note that HttpClient should be reused to optimize resources.</param>
        /// <param name="deEntitize">Function for removing HTML entities. You can use HtmlAgilityPack for this. Optional - can be left null.</param>
        public NicoApiClient(Func<string, string> deEntitize = null, HttpClient httpClient = null) {
            _httpClient = httpClient;
            _deEntitize = deEntitize;
        }

        private readonly Func<string, string> _deEntitize;
        private readonly HttpClient _httpClient;

        private HttpClient HttpClient => _httpClient ?? new HttpClient();

        public async Task<VideoDataResult> GetTitleAPIAsync(string id) {

            var url = string.Format("https://ext.nicovideo.jp/api/getthumbinfo/{0}", id);

            NicoResponse nicoResponse;

            try {
                nicoResponse = await XmlRequest.GetXmlObjectAsync<NicoResponse>(url, HttpClient);
            } catch (HttpRequestException x) {
                throw new NicoApiException("Unable to query data.", x);
            }

            return ParseResponse(nicoResponse);

        }

        /// <summary>
        /// Parse length into seconds.
        /// </summary>
        /// <param name="lengthStr">Length from Nico response, for example 2:58.</param>
        /// <returns>Length in seconds. Can be null if <paramref name="lengthStr"/> could not be parsed.</returns>
        public static int? ParseLength(string lengthStr) {

            if (string.IsNullOrEmpty(lengthStr))
                return null;

            var parts = lengthStr.Split(':');

            if (parts.Length != 2)
                return null;

            if (!int.TryParse(parts[0], out int min) || !int.TryParse(parts[1], out int sec))
                return null;

            var totalSec = min*60 + sec;

            return totalSec;

        }

        /// <summary>
        /// Parses a video post by URL.
        /// </summary>
        /// <param name="url">NicoNicoDouga video URL, for example https://www.nicovideo.jp/watch/sm1234567 </param>
        /// <returns>Result of the parse operation if successful. Cannot be null.</returns>
        /// <exception cref="NicoApiException">If video data could not be loaded.</exception>
        public async Task<VideoDataResult> ParseByUrlAsync(string url) {
            var id = NicoUrlHelper.GetIdByUrl(url) ?? throw new NicoApiException("Not a valid NicoNicoDouga URL.");
            return await GetTitleAPIAsync(id);
        }

        public VideoDataResult ParseResponse(NicoResponse nicoResponse) {

            if (nicoResponse == null)
                throw new ArgumentNullException(nameof(nicoResponse));
			
            if (nicoResponse.Status == "fail") {
                var err = (nicoResponse.Error != null ? nicoResponse.Error.Description : "empty response");
                throw new NicoApiException(string.Format("API response contained an error: {0}.", err), err);
            }

            var thumb = nicoResponse.Thumb;
            var title = DeEntitize(nicoResponse.Thumb.Title);
            var thumbUrl = UrlHelper.UpgradeToHttps(nicoResponse.Thumb.Thumbnail_Url) ?? string.Empty;
            var userId = nicoResponse.Thumb.User_Id ?? string.Empty;
            var length = ParseLength(nicoResponse.Thumb.Length);
            var author = nicoResponse.Thumb.User_Nickname;
            var publishDate = ParseDate(nicoResponse.Thumb.First_Retrieve);
            var tags = thumb.Tags.Select(tag => new NicoTag(tag.Name, tag.Lock)).ToArray();
            var description = nicoResponse.Thumb.Description;

            var result = new VideoDataResult(title, thumbUrl, length, publishDate, thumb.ViewCount, userId, author, tags, description);

            return result;

        }

        private string DeEntitize(string htmlStr) => _deEntitize?.Invoke(htmlStr) ?? htmlStr;

        private static DateTimeOffset? ParseDate(string dateStr) => DateTimeOffset.TryParse(dateStr, out var date) ? (DateTimeOffset?)date : null;

	}

}
