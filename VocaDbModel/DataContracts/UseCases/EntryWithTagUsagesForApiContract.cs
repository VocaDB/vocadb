using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.UseCases;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record EntryWithTagUsagesForApiContract
{
    public EntryWithTagUsagesForApiContract(
        IEntryWithStatus entry,
        IEnumerable<TagUsage> tagUsages,
        ContentLanguagePreference languagePreference,
        IUserPermissionContext userContext,
        IUserIconFactory userIconFactory
    )  {
        CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, entry);
        TagUsages = tagUsages.Select(u => new TagUsageWithVotesForApiContract(u, languagePreference, userIconFactory)).ToArray();
    }

    [DataMember]
    public bool CanRemoveTagUsages { get; init; }

    [DataMember]
    public TagUsageWithVotesForApiContract[] TagUsages { get; init; }
}