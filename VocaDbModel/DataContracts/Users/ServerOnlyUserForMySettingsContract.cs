#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.DataContracts.Users
{
	public class ServerOnlyUserForMySettingsContract : ServerOnlyUserContract
	{
		public ServerOnlyUserForMySettingsContract()
		{
			AboutMe = Location = string.Empty;
			WebLinks = new WebLinkContract[] { };
		}

		public ServerOnlyUserForMySettingsContract(User user)
			: base(user, true)
		{
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
			Stylesheet = user.Options.Stylesheet;
			TwitterId = user.Options.TwitterId;
			TwitterName = user.Options.TwitterName;
			UnreadNotificationsToKeep = user.Options.UnreadNotificationsToKeep;
			WebLinks = user.WebLinks.OrderBy(w => w.DescriptionOrUrl).Select(w => new WebLinkContract(w)).ToArray();
		}

		public string AboutMe { get; init; }

		public bool CanChangeName { get; init; }

		public bool EmailVerified { get; init; }

		public string HashedAccessKey { get; init; }

		public bool HasPassword { get; init; }

		public bool HasTwitterToken { get; init; }

		public UserKnownLanguageContract[] KnownLanguages { get; init; }

		public string Location { get; init; }

		public bool PublicRatings { get; init; }

		public bool ShowChatbox { get; init; }

		public string Stylesheet { get; init; }

		public int TwitterId { get; init; }

		public string TwitterName { get; init; }

		public int UnreadNotificationsToKeep { get; init; }

		public WebLinkContract[] WebLinks { get; init; }
	}
}
