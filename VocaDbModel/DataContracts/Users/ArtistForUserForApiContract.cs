using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users;

[DataContract]
public class ArtistForUserForApiContract
{
#nullable disable
	public ArtistForUserForApiContract() { }
#nullable enable

	public ArtistForUserForApiContract(
		IArtistForUser? artistForUser,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory? thumbPersister,
		ArtistOptionalFields includedFields
	)
	{
		Artist = artistForUser is not null
			? new ArtistForApiContract(artistForUser.Artist, languagePreference, permissionContext, thumbPersister, includedFields)
			: null;
		Id = artistForUser?.Id ?? 0;
	}

	[DataMember]
	public ArtistForApiContract? Artist { get; init; }

	[DataMember]
	public int Id { get; init; }
}
