#nullable disable

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// API queries for PVs
	/// </summary>
	[RoutePrefix("api/pvs")]
	public class PVApiController : ApiController
	{
		private readonly IPVParser _pvParser;
		private readonly IUserPermissionContext _permissionContext;
		private readonly PVQueries _queries;

		public PVApiController(IPVParser pvParser, IUserPermissionContext permissionContext, PVQueries queries)
		{
			_pvParser = pvParser;
			_permissionContext = permissionContext;
			_queries = queries;
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
		[Route("for-songs")]
		public PartialFindResult<PVForSongContract> GetList(string name = null, string author = null,
			PVService? service = null,
			int maxResults = 10, bool getTotalCount = false,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) => _queries.GetList(name, author, service, maxResults, getTotalCount, lang);

		[Route("")]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<PVContract> GetPVByUrl(string pvUrl, PVType type = PVType.Original, bool getTitle = true)
		{
			if (string.IsNullOrEmpty(pvUrl))
				throw new HttpResponseException(HttpStatusCode.BadRequest);

			var result = await _pvParser.ParseByUrlAsync(pvUrl, getTitle, _permissionContext);

			if (!result.IsOk)
			{
				var msg = result.Exception.Message;
				throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
				{
					ReasonPhrase = msg,
					Content = new StringContent(msg)
				});
			}

			var contract = new PVContract(result, type);
			return contract;
		}
	}
}