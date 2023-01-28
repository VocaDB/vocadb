#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users;

[Obsolete]
[DataContract(Namespace = Schemas.VocaDb, Name = "UpdateUserSettingsContract")]
public class ServerOnlyUpdateUserSettingsContract : ServerOnlyUserContract
{
	public ServerOnlyUpdateUserSettingsContract() { }

	public ServerOnlyUpdateUserSettingsContract(User user)
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

	public string AboutMe { get; init; }

	public string Location { get; init; }

	public UserKnownLanguageContract[] KnownLanguages { get; init; }

	public string NewPass { get; init; }

	public string OldPass { get; init; }

	public bool PublicRatings { get; init; }

	public bool ShowChatbox { get; init; }

	public string Stylesheet { get; init; }

	public int UnreadNotificationsToKeep { get; init; }

	public WebLinkContract[] WebLinks { get; init; }
}
