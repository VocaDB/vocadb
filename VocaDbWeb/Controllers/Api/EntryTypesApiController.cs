using System.Web.Http;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// Gets information about <see cref="EntryType"/>.
	/// </summary>
	[RoutePrefix("api/entry-types")]
	public class EntryTypesApiController : ApiController {

		public EntryTypesApiController(TagQueries queries) {
			tagQueries = queries;
		}

		private readonly TagQueries tagQueries;

		[Route("{entryType}/{subType?}/tag")]
		public TagForApiContract GetMappedTag(EntryType entryType, string subType = null, TagOptionalFields fields = TagOptionalFields.None)
			=> tagQueries.FindTagForEntryType(new EntryTypeAndSubType(entryType, subType), (tag, lang) => new TagForApiContract(tag, lang, fields), true);

	}

}