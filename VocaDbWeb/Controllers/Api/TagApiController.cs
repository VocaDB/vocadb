using System;
using System.Linq;
using System.Web.Http;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Web.Controllers.DataAccess;
using VocaDb.Web.Helpers;
using WebApi.OutputCache.V2;

namespace VocaDb.Web.Controllers.Api {
	
	/// <summary>
	/// API queries for tags.
	/// </summary>
	[RoutePrefix("api/tags")]
	public class TagApiController : ApiController {

		private const int absoluteMax = 50;
		private const int defaultMax = 10;
		private readonly TagQueries queries;
		private readonly IEntryImagePersisterOld thumbPersister;

		public TagApiController(TagQueries queries, IEntryImagePersisterOld thumbPersister) {
			this.queries = queries;
			this.thumbPersister = thumbPersister;
		}

		/// <summary>
		/// Gets a list of tag category names.
		/// </summary>
		[Route("categoryNames")]
		public string[] GetCategoryNamesList() {
		
			return queries.HandleQuery(ctx => ctx.Query().Where(t => t.CategoryName != null && t.CategoryName != "").Select(t => t.CategoryName).Distinct().ToArray());

		}

		/// <summary>
		/// Find tags.
		/// </summary>
		/// <param name="query">Tag name query (optional).</param>
		/// <param name="allowAliases">Whether to allow tag alises. If this is false, alises will not be included.</param>
		/// <param name="categoryName">Filter tags by category (optional). If specified, this must be an exact match (case insensitive).</param>
		/// <param name="start">First item to be retrieved (optional, defaults to 0).</param>
		/// <param name="maxResults">Maximum number of results to be loaded (optional, defaults to 10, maximum of 30).</param>
		/// <param name="getTotalCount">Whether to load total number of items (optional, default to false).</param>
		/// <param name="nameMatchMode">Match mode for song name (optional, defaults to Exact).</param>
		/// <param name="fields">
		/// List of optional fields (optional). Possible values are Description, MainPicture.
		/// </param>
		/// <returns>Page of tags.</returns>
		/// <example>http://vocadb.net/api/tags?query=voca&amp;nameMatchMode=StartsWith</example>
		[Route("")]
		public PartialFindResult<TagForApiContract> GetList(
			string query = "",
			bool allowAliases = false,
			string categoryName = "",
			int start = 0, int maxResults = defaultMax, bool getTotalCount = false,
			NameMatchMode nameMatchMode = NameMatchMode.Exact,
			TagOptionalFields fields = TagOptionalFields.None) {
			
			maxResults = Math.Min(maxResults, absoluteMax);
			var ssl = WebHelper.IsSSL(Request);
			var queryParams = new CommonSearchParams(query, false, nameMatchMode, false, false);
			var paging = new PagingProperties(start, maxResults, getTotalCount);

			var tags = queries.Find(t => new TagForApiContract(t, thumbPersister, ssl, fields), 
				queryParams, paging, 
				allowAliases, categoryName);

			return tags;

		}

		/// <summary>
		/// Find tag names by a part of name.
		/// 
		/// Matching is done anywhere from the name.
		/// Spaces are automatically converted into underscores.
		/// </summary>
		/// <param name="query">Tag name query, for example "rock".</param>
		/// <param name="allowAliases">
		/// Whether to find tags that are aliases of other tags as well. 
		/// If false, only tags that are not aliases will be listed.
		/// </param>
		/// <param name="maxResults">Maximum number of search results.</param>
		/// <returns>
		/// List of tag names, for example "vocarock", matching the query. Cannot be null.
		/// </returns>
		[Route("names")]
		public string[] GetNames(
			string query = "", bool allowAliases = true,
			int maxResults = 10) {
			
			return queries.FindNames(query, allowAliases, true, maxResults);

		}

		[Route("top")]
		[CacheOutput(ClientTimeSpan = 86400, ServerTimeSpan = 86400)]
		public string[] GetTopTags(string categoryName) {
			
			return queries.HandleQuery(ctx => {
				
				var tags = ctx.Query()
					.Where(t => categoryName == null || t.CategoryName == categoryName)
					.OrderByDescending(t => t.AllAlbumTagUsages.Count + t.AllArtistTagUsages.Count + t.AllSongTagUsages.Count)
					.Select(t => t.Name)
					.Take(15)
					.ToArray()
					.OrderBy(t => t)
					.ToArray();

				return tags;

			});

		}

	}

}