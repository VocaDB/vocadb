using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record TagUsageWithVotesForApiContract
{
    [DataMember]
    public DateTime Date { get; init; }

    [DataMember]
    public int Count { get; init; }

    [DataMember]
    public long Id { get; init; }

    [DataMember]
    public TagBaseContract Tag { get; init; }

    [DataMember]
    public UserForApiContract[] Votes {get; init;}

    public TagUsageWithVotesForApiContract(TagUsage usage, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory) {
        Count = usage.Count;
        Date = usage.Date;
        Id = usage.Id;
        Tag = new TagBaseContract(usage.Tag, languagePreference);

        Votes= usage.VotesBase.Select(v => new UserForApiContract(v.User, userIconFactory, UserOptionalFields.MainPicture)).ToArray();
    }
}