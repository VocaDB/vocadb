using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Api;

/// <summary>
/// Creates instances of <see cref="EntryForApiContract"/>.
/// </summary>
public class EntryForApiContractFactory
{
	private readonly IAggregatedEntryImageUrlFactory _thumbPersister;
	private readonly IUserPermissionContext _permissionContext;

	public EntryForApiContractFactory(
		IAggregatedEntryImageUrlFactory thumbPersister,
		IUserPermissionContext permissionContext
	)
	{
		_thumbPersister = thumbPersister;
		_permissionContext = permissionContext;
	}

	public EntryForApiContract Create(
		IEntryWithNames entry,
		EntryOptionalFields includedFields,
		ContentLanguagePreference languagePreference
	)
	{
		return EntryForApiContract.Create(entry, languagePreference, _permissionContext, _thumbPersister, includedFields);
	}
}
