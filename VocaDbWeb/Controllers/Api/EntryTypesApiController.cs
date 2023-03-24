using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code.Security;
using ApiController = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace VocaDb.Web.Controllers.Api;

/// <summary>
/// Gets information about <see cref="EntryType"/>.
/// </summary>
[EnableCors(AuthenticationConstants.WebApiCorsPolicy)]
[Route("api/entry-types")]
[ApiController]
public class EntryTypesApiController : ApiController
{
	private readonly TagQueries _tagQueries;
	private readonly IUserPermissionContext _permissionContext;

	public EntryTypesApiController(
		TagQueries queries,
		IUserPermissionContext permissionContext
	)
	{
		_tagQueries = queries;
		_permissionContext = permissionContext;
	}

	[HttpGet("{entryType}/{subType?}/tag")]
	public TagForApiContract GetMappedTag(
		EntryType entryType,
		string subType = null,
		TagOptionalFields fields = TagOptionalFields.None
	)
	{
		return _tagQueries.FindTagForEntryType(
			new EntryTypeAndSubType(entryType, subType),
			(tag, lang) => new TagForApiContract(tag, lang, _permissionContext, fields),
			true
		);
	}
}