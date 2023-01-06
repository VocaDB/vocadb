#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users;

/// <summary>
/// User with additional permission flags and artist ownership permissions.
/// Used for user details page as well as checking for permissions of the logged in user.
/// </summary>
[DataContract(Namespace = Schemas.VocaDb, Name = "UserWithPermissionsContract")]
public class ServerOnlyUserWithPermissionsContract : ServerOnlyUserContract
{
	public ServerOnlyUserWithPermissionsContract() { }

	public ServerOnlyUserWithPermissionsContract(User user, ContentLanguagePreference languagePreference, bool getPublicCollection = false)
		: base(user, getPublicCollection)
	{
		AdditionalPermissions = new HashSet<PermissionToken>(user.AdditionalPermissions.PermissionTokens);
		AlbumFormatString = user.Options.AlbumFormatString;
		EffectivePermissions = new HashSet<PermissionToken>(user.EffectivePermissions.PermissionTokens);
		OwnedArtistEntries = user.OwnedArtists.Select(a => new ArtistForUserContract(a, languagePreference)).ToArray();
		Poisoned = user.Options.Poisoned;
		Stylesheet = user.Options.Stylesheet;
		Supporter = user.Options.Supporter;
	}

	[DataMember]
	public HashSet<PermissionToken> AdditionalPermissions { get; set; }

#nullable enable
	[DataMember]
	public string AlbumFormatString { get; init; }
#nullable disable

	[DataMember]
	public HashSet<PermissionToken> EffectivePermissions { get; init; }

	/// <summary>
	/// List of artist entries owned by the user. Cannot be null.
	/// </summary>
	[DataMember]
	public ArtistForUserContract[] OwnedArtistEntries { get; init; }

	[DataMember]
	public bool Poisoned { get; init; }

	[DataMember]
	public string Stylesheet { get; init; }

	[DataMember]
	public bool Supporter { get; init; }

	[DataMember]
	public int UnreadMessagesCount { get; set; }
}
