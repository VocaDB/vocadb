using System.Runtime.Caching;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

/// <summary>
/// API queries for PVs
/// </summary>
[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/pvs")]
[ApiController]
public class PVApiController : ApiController
{
	private readonly ObjectCache _cache;
	private readonly IPVParser _pvParser;
	private readonly HttpClient _client;
	private readonly IUserPermissionContext _permissionContext;
	private readonly PVQueries _queries;

	public PVApiController(IPVParser pvParser, IUserPermissionContext permissionContext, PVQueries queries, HttpClient client, ObjectCache cache)
	{
		_pvParser = pvParser;
		_permissionContext = permissionContext;
		_queries = queries;
		_client = client;
		_cache = cache;
	}

	/// <summary>
	/// Gets a list of PVs for songs.
	/// </summary>
	/// <param name="name">PV title (optional).</param>
	/// <param name="author">Uploader name (optional).</param>
	/// <param name="service">PV service (optional).</param>
	/// <param name="maxResults">Maximum number of results.</param>
	/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
	/// <param name="lang">Content language preference (optional).</param>
	/// <returns>List of PVs.</returns>
	[HttpGet("for-songs")]
	public PartialFindResult<PVForSongContract> GetList(
		string? name = null,
		string? author = null,
		PVService? service = null,
		int maxResults = 10,
		bool getTotalCount = false,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	) => _queries.GetList(name, author, service, maxResults, getTotalCount, lang);

	[HttpGet("thumbnail")]
	public async Task<ActionResult> GetNNDThumbnai(string pvUrl)
	{
		if (!VideoServiceHelper.Services.Any(s => s.IsValidFor(pvUrl)))
		{
			return BadRequest();
		}

		var ogImage = await _cache.GetOrInsertAsync(pvUrl, CachePolicy.Never(), async () =>
		{
			return await GetOgImage(pvUrl) ?? string.Empty;
		});

		// Recheck if cached item is null
		if (string.IsNullOrEmpty(ogImage))
		{
			return NotFound();
		}

		if (ogImage == null)
		{
			return BadRequest();
		}

		return RedirectPermanent(ogImage);
	}

	// TODO: Move this code in a separate class
	private async Task<string?> GetOgImage(string pvUrl)
	{

		var response = await _client.GetAsync(pvUrl);

		if (response.IsSuccessStatusCode)
		{
			var html = await response.Content.ReadAsStringAsync();
			var ogImage = ParseOgImage(html);

			if (!string.IsNullOrEmpty(ogImage))
			{
				return ogImage;
			}
		}

		return null;
	}

	private string? ParseOgImage(string html)
	{
		var htmlDocument = new HtmlDocument();
		htmlDocument.LoadHtml(html);

		var ogImageNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
		var ogImage = ogImageNode?.GetAttributeValue("content", null);

		// Fallback for piapro
		var twitterImageNode = htmlDocument.DocumentNode.SelectSingleNode("//meta[@name='twitter:image']");
		var twitterImage = twitterImageNode?.GetAttributeValue("content", null);

		return ogImage ?? twitterImage;
	}

	[HttpGet("")]
	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task<ActionResult<PVContract>> GetPVByUrl(string pvUrl, PVType type = PVType.Original, bool getTitle = true)
	{
		if (string.IsNullOrEmpty(pvUrl))
			return BadRequest();

		var result = await _pvParser.ParseByUrlAsync(pvUrl, getTitle, _permissionContext);

		if (!result.IsOk)
		{
			var msg = result.Exception.Message;
			return BadRequest(msg);
		}

		var contract = new PVContract(result, type);
		return contract;
	}
}