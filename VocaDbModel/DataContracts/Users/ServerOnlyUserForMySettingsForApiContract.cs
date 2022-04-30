using System.Runtime.Serialization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Security;

namespace VocaDb.Model.DataContracts.Users;

/// <summary>
/// Data contract for <see cref="User"/> with most properties.
/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
/// </summary>
[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ServerOnlyUserForMySettingsForApiContract
{
	[DataMember]
	public string AboutMe { get; init; }

	[DataMember]
	public bool AnonymousActivity { get; init; }

	[DataMember]
	public bool CanChangeName { get; init; }

	[DataMember]
	public string Culture { get; set; }

	[DataMember]
	public ContentLanguagePreference DefaultLanguageSelection { get; set; }

	[DataMember]
	public string Email { get; init; }

	[DataMember]
	public UserEmailOptions EmailOptions { get; init; }

	[DataMember]
	public bool EmailVerified { get; init; }

	[DataMember]
	public string HashedAccessKey { get; init; }

	[DataMember]
	public bool HasPassword { get; init; }

	[DataMember]
	public bool HasTwitterToken { get; init; }

	[DataMember]
	public int Id { get; set; }

	[DataMember]
	public UserKnownLanguageContract[] KnownLanguages { get; init; }

	[DataMember]
	public string Language { get; set; }

	[DataMember]
	public string Location { get; init; }

	/// <summary>
	/// Can be null.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; set; }

	[DataMember]
	public PVService PreferredVideoService { get; init; }

	[DataMember]
	public bool PublicAlbumCollection { get; init; }

	[DataMember]
	public bool PublicRatings { get; init; }

	[DataMember]
	public bool ShowChatbox { get; init; }

	[DataMember]
	public string? Stylesheet { get; init; }

	[DataMember]
	public string TwitterName { get; init; }

	[DataMember]
	public int UnreadNotificationsToKeep { get; init; }

	[DataMember]
	public WebLinkContract[] WebLinks { get; init; }

	public ServerOnlyUserForMySettingsForApiContract(User user, IUserIconFactory? iconFactory)
	{
		AboutMe = user.Options.AboutMe;
		AnonymousActivity = user.AnonymousActivity;
		CanChangeName = user.CanChangeName;
		Culture = user.Culture;
		DefaultLanguageSelection = user.DefaultLanguageSelection;
		Email = user.Email;
		EmailOptions = user.EmailOptions;
		EmailVerified = user.Options.EmailVerified;
		HashedAccessKey = LoginManager.GetHashedAccessKey(user.AccessKey);
		HasPassword = !string.IsNullOrEmpty(user.Password);
		HasTwitterToken = !string.IsNullOrEmpty(user.Options.TwitterOAuthToken);
		Id = user.Id;
		KnownLanguages = user.KnownLanguages
			.Select(l => new UserKnownLanguageContract(l))
			.ToArray();
		Language = user.Language.CultureCode;
		Location = user.Options.Location;
		MainPicture = iconFactory?.GetUserIcons(user, ImageSizes.All);
		Name = user.Name;
		PreferredVideoService = user.PreferredVideoService;
		PublicAlbumCollection = user.Options.PublicAlbumCollection;
		PublicRatings = user.Options.PublicRatings;
		ShowChatbox = user.Options.ShowChatbox;
		Stylesheet = user.Options.Stylesheet;
		TwitterName = user.Options.TwitterName;
		UnreadNotificationsToKeep = user.Options.UnreadNotificationsToKeep;
		WebLinks = user.WebLinks
			.OrderBy(w => w.DescriptionOrUrl)
			.Select(w => new WebLinkContract(w))
			.ToArray();
	}
}
