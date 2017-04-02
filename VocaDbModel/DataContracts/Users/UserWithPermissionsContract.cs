using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// User with additional permission flags and artist ownership permissions.
	/// Used for user details page as well as checking for permissions of the logged in user.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserWithPermissionsContract : UserContract {

		public UserWithPermissionsContract() { }

		public UserWithPermissionsContract(User user, ContentLanguagePreference languagePreference, bool getPublicCollection = false)
			: base(user, getPublicCollection) {

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

		[DataMember]
		public string AlbumFormatString { get; set; }

		[DataMember]
		public HashSet<PermissionToken> EffectivePermissions { get; set; }

		/// <summary>
		/// List of artist entries owned by the user. Cannot be null.
		/// </summary>
		[DataMember]
		public ArtistForUserContract[] OwnedArtistEntries { get; set; }

		[DataMember]
		public bool Poisoned { get; set; }

		[DataMember]
		public string Stylesheet { get; set; }

		[DataMember]
		public bool Supporter { get; set; }

		[DataMember]
		public int UnreadMessagesCount { get; set; }

	}

}
