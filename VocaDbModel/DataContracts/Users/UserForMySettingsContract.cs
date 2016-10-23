using System.Linq;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.DataContracts.Users {

	public class UserForMySettingsContract : UserContract {

		public UserForMySettingsContract() {
			AboutMe = Location = string.Empty;
			WebLinks = new WebLinkContract[] {};
		}

		public UserForMySettingsContract(User user)
			: base(user, true) {

			AboutMe = user.Options.AboutMe;
			CanChangeName = user.CanChangeName;
			EmailVerified = user.Options.EmailVerified;
			HashedAccessKey = LoginManager.GetHashedAccessKey(user.AccessKey);
			HasPassword = !string.IsNullOrEmpty(user.Password);
			HasTwitterToken = !string.IsNullOrEmpty(user.Options.TwitterOAuthToken);
			KnownLanguages = user.KnownLanguages.Select(l => new UserKnownLanguageContract(l)).ToArray();
			Location = user.Options.Location;
			PublicRatings = user.Options.PublicRatings;
			ShowChatbox = user.Options.ShowChatbox;
			TwitterId = user.Options.TwitterId;
			TwitterName = user.Options.TwitterName;
			UnreadNotificationsToKeep = user.Options.UnreadNotificationsToKeep;
			WebLinks = user.WebLinks.OrderBy(w => w.DescriptionOrUrl).Select(w => new WebLinkContract(w)).ToArray();

		}

		public string AboutMe { get; set; }

		public bool CanChangeName { get; set; }

		public bool EmailVerified { get; set; }

		public string HashedAccessKey { get; set; }

		public bool HasPassword { get; set; }

		public bool HasTwitterToken { get; set; }

		public UserKnownLanguageContract[] KnownLanguages { get; set; }

		public string Location { get; set; }

		public bool PublicRatings { get; set; }

		public bool ShowChatbox { get; set; }

		public int TwitterId { get; set; }

		public string TwitterName { get; set; }

		public int UnreadNotificationsToKeep { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}
}
