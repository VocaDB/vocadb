using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	/// <summary>
	/// User with profile icon URL.
	/// Contains no sensitive information.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UserWithIconContract : UserBaseContract, IUserWithIcon {

		public UserWithIconContract() { }

		public UserWithIconContract(IUser user, string iconUrl)
			: base(user) {

			IconUrl = iconUrl;

		}

		public UserWithIconContract(User user)
			: this(user, string.Empty) {}

		public UserWithIconContract(IUserWithEmail user, IUserIconFactory iconFactory)
			: this(user, iconFactory != null ? iconFactory.GetIconUrl(user) : string.Empty) {}

		/// <summary>
		/// Initializes user contract with fallback name.
		/// </summary>
		/// <param name="user">User. Can be null.</param>
		/// <param name="fallbackName">Fallback name if <see cref="user"/> is null. Cannot be null.</param>
		/// <param name="iconFactory">User icon factory. Cannot be null.</param>
		public UserWithIconContract(User user, string fallbackName, IUserIconFactory iconFactory)
			: base(user, fallbackName) {

			if (user != null && iconFactory != null) {
				IconUrl = iconFactory.GetIconUrl(user);
			}

		}

		[DataMember]
		public string IconUrl { get; set; }

	}

}
