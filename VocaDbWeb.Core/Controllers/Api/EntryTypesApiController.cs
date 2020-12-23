#nullable disable

using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api
{
	/// <summary>
	/// Gets information about <see cref="EntryType"/>.
	/// </summary>
	[Route("api/entry-types")]
	[ApiController]
	public class EntryTypesApiController : ApiController
	{
		public EntryTypesApiController(TagQueries queries)
		{
			_tagQueries = queries;
		}

		private readonly TagQueries _tagQueries;

		[HttpGet("{entryType}/{subType?}/tag")]
		public TagForApiContract GetMappedTag(EntryType entryType, string subType = null, TagOptionalFields fields = TagOptionalFields.None)
			=> _tagQueries.FindTagForEntryType(new EntryTypeAndSubType(entryType, subType), (tag, lang) => new TagForApiContract(tag, lang, fields), true);
	}
}