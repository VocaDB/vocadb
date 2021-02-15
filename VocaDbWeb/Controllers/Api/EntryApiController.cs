#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search;
using VocaDb.Web.Helpers;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// Controller for managing base class for common entries.
	/// </summary>
	[Route("api/entries")]
	[ApiController]
	public class EntryApiController : ApiController
	{
		private const int AbsoluteMax = 50;
		private const int DefaultMax = 10;

		private readonly AlbumService _albumService;
		private readonly ArtistService _artistService;
		private readonly IEntryUrlParser _entryUrlParser;
		private readonly OtherService _otherService;
		private readonly EntryQueries _queries;
		private readonly SongQueries _songQueries;
		private readonly IViewRenderService _viewRenderService;

		private int GetMaxResults(int max) => Math.Min(max, AbsoluteMax);

		public EntryApiController(
			EntryQueries queries,
			OtherService otherService,
			AlbumService albumService,
			ArtistService artistService,
			SongQueries songQueries,
			IEntryUrlParser entryUrlParser,
			IViewRenderService viewRenderService)
		{
			_queries = queries;
			_otherService = otherService;
			_albumService = albumService;
			_artistService = artistService;
			_songQueries = songQueries;
			_entryUrlParser = entryUrlParser;
			_viewRenderService = viewRenderService;
		}

		/// <summary>
		/// Find entries.
		/// </summary>
		/// <param name="query">Entry name query (optional).</param>
		/// <param name="tagName">Filter by tag name (optional).</param>
		/// <param name="tagId">Filter by tag Id (optional).</param>
		/// <param name="childTags">Include child tags, if the tags being filtered by have any.</param>
		/// <param name="status">Filter by entry status (optional).</param>
		/// <param name="entryTypes">Included entry types (optional).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="sort">Sort rule (optional, defaults to Name). Possible values are None, Name, AdditionDate.</param>
		/// <param name="nameMatchMode">Match mode for entry name (optional, defaults to Exact).</param>
		/// <param name="fields">List of optional fields (optional). Possible values are Description, MainPicture, Names, Tags, WebLinks.</param>
		/// <param name="lang">Content language preference (optional).</param>
		/// <returns>Page of entries.</returns>
		/// <example>http://vocadb.net/api/entries?query=164&amp;fields=MainPicture</example>
		[HttpGet("")]
		public PartialFindResult<EntryForApiContract> GetList(
			string query = "",
			[FromQuery(Name = "tagName[]")] string[] tagName = null,
			[FromQuery(Name = "tagId[]")] int[] tagId = null,
			bool childTags = false,
			EntryTypes? entryTypes = null,
			EntryStatus? status = null,
			int start = 0, int maxResults = DefaultMax, bool getTotalCount = false,
			EntrySortRule sort = EntrySortRule.Name,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			EntryOptionalFields fields = EntryOptionalFields.None,
			ContentLanguagePreference lang = ContentLanguagePreference.Default
			)
		{
			maxResults = GetMaxResults(maxResults);

			return _queries.GetList(query, tagId, tagName, childTags, status, entryTypes,
				start, maxResults, getTotalCount, sort, nameMatchMode, fields, lang, searchEvents: true);
		}

		/// <summary>
		/// Gets a list of entry names. Ideal for autocomplete boxes.
		/// </summary>
		/// <param name="query">Text query.</param>
		/// <param name="nameMatchMode">Name match mode.</param>
		/// <param name="maxResults">Maximum number of results.</param>
		/// <returns>List of entry names.</returns>
		[HttpGet("names")]
		public string[] GetNames(string query = "", NameMatchMode nameMatchMode = NameMatchMode.Auto, int maxResults = 10) => _otherService.FindNames(SearchTextQuery.Create(query, nameMatchMode), maxResults);

		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpGet("tooltip")]
		public async Task<ActionResult<string>> GetToolTip(string url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return BadRequest("URL must be specified");

			var entryId = _entryUrlParser.Parse(url, allowRelative: true);

			if (entryId.IsEmpty)
				return BadRequest("Invalid URL");

			var data = string.Empty;
			var id = entryId.Id;

			switch (entryId.EntryType)
			{
				case EntryType.Album:
					data = await _viewRenderService.RenderToStringAsync("AlbumWithCoverPopupContent", _albumService.GetAlbum(id));
					break;
				case EntryType.Artist:
					data = await _viewRenderService.RenderToStringAsync("ArtistPopupContent", _artistService.GetArtist(id));
					break;
				case EntryType.Song:
					data = await _viewRenderService.RenderToStringAsync("SongPopupContent", _songQueries.GetSong(id));
					break;
			}

			return data;
		}
	}
}