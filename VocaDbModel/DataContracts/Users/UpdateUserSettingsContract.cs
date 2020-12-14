#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class UpdateUserSettingsContract : UserContract
	{
		public UpdateUserSettingsContract() { }

		public UpdateUserSettingsContract(User user)
			: base(user, true)
		{
			AboutMe = user.Options.AboutMe;
			Location = user.Options.Location;
			KnownLanguages = user.KnownLanguages.Select(l => new UserKnownLanguageContract(l)).ToArray();
			PublicRatings = user.Options.PublicRatings;
			ShowChatbox = user.Options.ShowChatbox;
			Stylesheet = user.Options.Stylesheet;
			UnreadNotificationsToKeep = user.Options.UnreadNotificationsToKeep;
			WebLinks = user.WebLinks.Select(w => new WebLinkContract(w)).ToArray();
		}

		public string AboutMe { get; set; }

		public string Location { get; set; }

		public UserKnownLanguageContract[] KnownLanguages { get; set; }

		public string NewPass { get; set; }

		public string OldPass { get; set; }

		public bool PublicRatings { get; set; }

		public bool ShowChatbox { get; set; }

		public string Stylesheet { get; set; }

		public int UnreadNotificationsToKeep { get; set; }

		public WebLinkContract[] WebLinks { get; set; }
	}
}
