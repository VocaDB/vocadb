using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Resources;

namespace VocaDb.Model.Service.EntryValidators;

public class ArtistValidator
{
	public bool IsValid(Artist artist)
	{
		ParamIs.NotNull(() => artist);

		var errors = new List<string>();

		if (artist.ArtistType == ArtistType.Unknown)
			errors.Add(ArtistValidationErrors.NeedType);

		if (artist.Names.Names.All(n => n.Language == ContentLanguageSelection.Unspecified))
			errors.Add(ArtistValidationErrors.UnspecifiedNames);

		if (artist.Description.IsEmpty && !artist.WebLinks.Any())
			errors.Add(ArtistValidationErrors.NeedReferences);

		return !errors.Any();
	}
}
