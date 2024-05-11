using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using Newtonsoft.Json;
using NLog;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices;

public class VideoServiceSoundCloud : VideoService
{
	private sealed record SoundCloudTokenResponse
	{
		public string Access_token { get; init; } = default!;
	}

	private sealed record SoundCloudUser
	{
		public string Avatar_url { get; init; } = default!;

		public string Permalink { get; init; } = default!;

		public string Username { get; init; } = default!;
	}

	private sealed record SoundCloudResult
	{
		public string Artwork_url { get; init; } = default!;

		public DateTime Created_at { get; init; }

		public int Duration { get; init; }

		public string Id { get; init; } = default!;

		public string Title { get; init; } = default!;

		public SoundCloudUser User { get; init; } = default!;
	}

	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

#nullable enable
	public VideoServiceSoundCloud(PVService service, IVideoServiceParser? parser, RegexLinkMatcher[] linkMatchers)
		: base(service, parser, linkMatchers) { }
#nullable disable

	public override string GetUrlById(string id, PVExtendedMetadata? extendedMetadata = null)
	{
		var compositeId = new SoundCloudId(id);
		var matcher = _linkMatchers.First();
		return $"http://{matcher.MakeLinkFromId(compositeId.SoundCloudUrl)}";
	}

	private static async Task<string> GetAccessToken()
	{
		s_log.Info("Requesting a new SoundCloud token");

		// Code from: https://github.com/TerribleDev/OwinOAuthProviders/blob/8b382b0429aeb656f54149ab6f3472dd559ae12f/src/Owin.Security.Providers.SoundCloud/SoundCloudAuthenticationHandler.cs#L74
		// TODO: Use IHttpClientFactory.
		using var httpClient = new HttpClient();

		var tokenResponse = await httpClient.PostAsync(
			requestUri: "https://api.soundcloud.com/oauth2/token",
			content: new FormUrlEncodedContent(new List<KeyValuePair<string?, string?>>
			{
				new("client_id", AppConfig.SoundCloudClientId),
				new("client_secret", AppConfig.SoundCloudClientSecret),
				new("grant_type", "client_credentials"),
			})
		);
		tokenResponse.EnsureSuccessStatusCode();

		var text = await tokenResponse.Content.ReadAsStringAsync();

		var response = JsonConvert.DeserializeObject<SoundCloudTokenResponse>(text);
		var accessToken = response.Access_token;

		return accessToken;
	}

	public async Task<VideoUrlParseResult> ParseBySoundCloudUrl(string url)
	{
		SslHelper.ForceStrongTLS();

		SoundCloudResult? result;

		bool HasStatusCode(WebException x, HttpStatusCode statusCode) => x.Response != null && ((HttpWebResponse)x.Response).StatusCode == statusCode;

		VideoUrlParseResult ReturnError(Exception? innerException, string? additionalInfo = null)
		{
			var msg = $"Unable to load SoundCloud URL '{url}'.{(additionalInfo != null ? " " + additionalInfo + "." : string.Empty)}";
			s_log.Warn(innerException, msg);
			return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, new VideoParseException(msg, innerException));
		}

		try
		{

			result = await JsonRequest.ReadObjectAsync<SoundCloudResult>(
				$"https://dream-traveler.fly.dev/soundcloud/api?url={url}",
				timeout: TimeSpan.FromSeconds(10)
			);
		}
		catch (WebException x) when (HasStatusCode(x, HttpStatusCode.Forbidden))
		{
			// Forbidden most likely means the artist has prevented API access to their tracks, http://stackoverflow.com/a/36529330
			return ReturnError(x, "This track cannot be embedded");
		}
		catch (WebException x) when (HasStatusCode(x, HttpStatusCode.NotFound))
		{
			return ReturnError(x, "Not found");
		}
		catch (WebException x)
		{
			return ReturnError(x);
		}
		catch (JsonSerializationException x)
		{
			return ReturnError(x);
		}
		catch (HttpRequestException x)
		{
			return ReturnError(x);
		}

		var trackId = result.Id;
		var title = result.Title;

		if (trackId == null || title == null)
			return VideoUrlParseResult.CreateError(url, VideoUrlParseResultType.LoadError, "Unable to load SoundCloud URL: Invalid response.");

		var author = result.User.Username;
		var length = result.Duration / 1000;

		var thumbUrl = result.Artwork_url;

		// Substitute song thumbnail with user avatar, if no actual thumbnail is provided. This is what the SoundCloud site does as well.
		if (string.IsNullOrEmpty(thumbUrl))
		{
			thumbUrl = result.User.Avatar_url;
		}

		var uploadDate = result.Created_at;

		var id = new SoundCloudId(trackId, url);
		var authorId = result.User.Permalink; // Using permalink because that's the public URL

		return VideoUrlParseResult.CreateOk(url, PVService.SoundCloud, id.ToString(), VideoTitleParseResult.CreateSuccess(title, author, authorId, thumbUrl, length, uploadDate: uploadDate));
	}

	public override Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle)
	{
		var soundCloudUrl = _linkMatchers[0].GetId(url);

		return ParseBySoundCloudUrl(soundCloudUrl);
	}

	public override IEnumerable<string> GetUserProfileUrls(string authorId)
	{
		return new[] {
			$"http://soundcloud.com/{authorId}",
			$"https://soundcloud.com/{authorId}",
		};
	}
}

/// <summary>
/// Composite SoundCloud ID. Contains both the track Id and the relative URL (for direct links).
/// </summary>
public class SoundCloudId
{
	/// <summary>
	/// Remove query string.
	/// See https://github.com/VocaDB/vocadb/issues/459
	/// </summary>
	private string CleanUrl(string url) => url.Split('?')[0];

	public SoundCloudId(string trackId, string soundCloudUrl)
	{
		ParamIs.NotNullOrEmpty(() => trackId);
		ParamIs.NotNullOrEmpty(() => soundCloudUrl);

		TrackId = trackId;
		SoundCloudUrl = CleanUrl(soundCloudUrl);
	}

	public SoundCloudId(string compositeId)
	{
		ParamIs.NotNull(() => compositeId);

		var parts = compositeId.Split(' ');

		if (parts.Length < 2)
		{
			throw new ArgumentException("Composite ID must contain both track Id and URL");
		}

		TrackId = parts[0];
		SoundCloudUrl = parts[1];
	}

	/// <summary>
	/// Relative URL, for example tamagotaso/nightcruise
	/// </summary>
	public string SoundCloudUrl { get; set; }

	/// <summary>
	/// Track ID, for example 8431571
	/// </summary>
	public string TrackId { get; set; }

	/// <summary>
	/// Gets the composite ID string with both the relative URL and track Id.
	/// </summary>
	/// <returns>Composite ID, for example "8431571 tamagotaso/nightcruise"</returns>
	public override string ToString()
	{
		return $"{TrackId} {SoundCloudUrl}";
	}
}
