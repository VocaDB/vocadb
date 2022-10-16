using System.Runtime.Serialization;
using FluentValidation;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users;

/// <summary>
/// Data contract for <see cref="User"/> with most properties.
/// SECURITY NOTE: take care when sending to client due to the contained sensitive information.
/// </summary>
[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ServerOnlyUpdateUserSettingsForApiContract
{
	[DataMember]
	public string AboutMe { get; init; }

	[DataMember]
	public bool AnonymousActivity { get; init; }

	[DataMember]
	public string Culture { get; init; }

	[DataMember]
	public ContentLanguagePreference DefaultLanguageSelection { get; init; }

	[DataMember]
	public string Email { get; init; }

	[DataMember]
	public UserEmailOptions EmailOptions { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public UserKnownLanguageContract[] KnownLanguages { get; init; }

	[DataMember]
	public string Language { get; init; }

	[DataMember]
	public string Location { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public string NewPass { get; init; }

	[DataMember]
	public string NewPassAgain { get; init; }

	[DataMember]
	public string OldPass { get; init; }

	[DataMember]
	public PVService PreferredVideoService { get; init; }

	[DataMember]
	public bool PublicAlbumCollection { get; init; }

	[DataMember]
	public bool PublicRatings { get; init; }

	[DataMember]
	public string? Stylesheet { get; init; }

	[DataMember]
	public int UnreadNotificationsToKeep { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public ServerOnlyUpdateUserSettingsForApiContract()
	{
		AboutMe = string.Empty;
		Culture = string.Empty;
		Email = string.Empty;
		KnownLanguages = Array.Empty<UserKnownLanguageContract>();
		Language = string.Empty;
		Location = string.Empty;
		Name = string.Empty;
		NewPass = string.Empty;
		NewPassAgain = string.Empty;
		OldPass = string.Empty;
		Stylesheet = string.Empty;
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public ServerOnlyUpdateUserSettingsForApiContract(User user)
	{
		AboutMe = user.Options.AboutMe;
		AnonymousActivity = user.AnonymousActivity;
		Culture = user.Culture;
		DefaultLanguageSelection = user.DefaultLanguageSelection;
		Email = user.Email;
		EmailOptions = user.EmailOptions;
		Id = user.Id;
		KnownLanguages = user.KnownLanguages
			.Select(l => new UserKnownLanguageContract(l))
			.ToArray();
		Language = user.Language.CultureCode;
		Location = user.Options.Location;
		Name = user.Name;
		NewPass = string.Empty;
		NewPassAgain = string.Empty;
		OldPass = string.Empty;
		PreferredVideoService = user.PreferredVideoService;
		PublicAlbumCollection = user.Options.PublicAlbumCollection;
		PublicRatings = user.Options.PublicRatings;
		Stylesheet = user.Options.Stylesheet;
		UnreadNotificationsToKeep = user.Options.UnreadNotificationsToKeep;
		WebLinks = user.WebLinks
			.Select(w => new WebLinkForApiContract(w))
			.ToArray();
	}
}

public sealed class ServerOnlyUpdateUserSettingsForApiContractValidator : AbstractValidator<ServerOnlyUpdateUserSettingsForApiContract>
{
	public ServerOnlyUpdateUserSettingsForApiContractValidator()
	{
		RuleFor(x => x.Location).MaximumLength(50);
		RuleFor(x => x.Email)/* TODO: .EmailAddress()*/.MaximumLength(50);
		RuleFor(x => x.Name).NotEmpty().Length(3, 100);
		RuleFor(x => x.OldPass).MaximumLength(100);
		When(x => !string.IsNullOrEmpty(x.NewPass), () =>
		{
			RuleFor(x => x.NewPass)
				.Equal(x => x.NewPassAgain).WithMessage("Passwords must match"/* TODO: ViewRes.User.MySettingsStrings.PasswordsMustMatch */)
				.Length(8, 100);
		});
		RuleFor(x => x.NewPassAgain).MaximumLength(100);
		RuleFor(x => x.UnreadNotificationsToKeep).GreaterThanOrEqualTo(1).LessThanOrEqualTo(390);
	}
}
