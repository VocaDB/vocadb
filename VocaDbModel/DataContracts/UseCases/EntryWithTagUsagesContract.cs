#nullable disable

using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.UseCases;

[Obsolete]
public class EntryWithTagUsagesContract : EntryBaseContract
{
	public EntryWithTagUsagesContract() { }

	public EntryWithTagUsagesContract(
		IEntryWithStatus entry,
		IEnumerable<TagUsage> tagUsages,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext userContext,
		IUserIconFactory userIconFactory
	)
		: base(entry)
	{
		CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, entry);
		TagUsages = tagUsages.Select(u => new TagUsageWithVotesContract(u, languagePreference, userIconFactory)).ToArray();
	}

	public bool CanRemoveTagUsages { get; init; }

	public TagUsageWithVotesContract[] TagUsages { get; init; }
}
