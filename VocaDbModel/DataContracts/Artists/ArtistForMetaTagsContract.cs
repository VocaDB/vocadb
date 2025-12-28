using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Artists;

/// <summary>
/// Minimal artist data contract for server-side rendering meta tags (OpenGraph, etc.).
/// Contains only the fields needed for SEO and social media previews.
/// </summary>
public sealed record ArtistForMetaTagsContract
{
	public ArtistType ArtistType { get; init; }

	public bool Deleted { get; init; }

	public EnglishTranslatedStringContract Description { get; init; }

	public EntryThumbForApiContract? MainPicture { get; init; }

	public string Name { get; init; }

	public DateTime? ReleaseDate { get; init; }

	public ArtistForApiContract[] VoiceProviders { get; init; }

	public ArtistForMetaTagsContract(
		Artist artist,
		ContentLanguagePreference languagePreference,
		IAggregatedEntryImageUrlFactory imageStore,
		IUserPermissionContext userContext
	)
	{
		ArtistType = artist.ArtistType;
		Deleted = artist.Deleted;
		Description = new EnglishTranslatedStringContract(artist.Description);
		Name = artist.TranslatedName[languagePreference];
		ReleaseDate = artist.ReleaseDate.DateTime;

		MainPicture = artist.Thumb is not null
			? new EntryThumbForApiContract(image: artist.Thumb, thumbPersister: imageStore)
			: null;

		// VoiceProviders are needed for the description generator
		VoiceProviders = artist
			.ArtistLinksOfType(ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne, allowInheritance: true)
			.Select(g => new ArtistForApiContract(
				artist: g,
				languagePreference: languagePreference,
				userContext,
				thumbPersister: null,
				includedFields: ArtistOptionalFields.None
			))
			.OrderBy(a => a.Name)
			.ToArray();
	}
}
