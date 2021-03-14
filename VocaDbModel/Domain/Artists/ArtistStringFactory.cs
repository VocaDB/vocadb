#nullable disable

using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Artists
{
	public class ArtistStringFactory
	{
		private readonly bool _allowRepeatingProducerAsPerformer;

		/// <summary>
		/// Gets the sort order for an artist in an artist string. 
		/// Determines the order the artist appear in the list.
		/// </summary>
		/// <param name="artistLink">Artist link. Cannot be null.</param>
		/// <param name="focus">Album focus.</param>
		/// <returns>Sort order, 0-based.</returns>
		private int GetSortOrderForArtistString(IArtistLinkWithRoles artistLink, ContentFocus focus)
		{
			var categories = ArtistHelper.GetCategories(artistLink);

			// Animator appears first for animation discs.
			if (focus == ContentFocus.Video && categories.HasFlag(ArtistCategories.Animator))
				return 0;

			if (focus == ContentFocus.Illustration && categories.HasFlag(ArtistCategories.Illustrator))
				return 0;

			// Composer role always appears first
			if (categories.HasFlag(ArtistCategories.Producer) && artistLink.Roles.HasFlag(ArtistRoles.Composer))
				return 1;

			// Other producers appear after composers
			if (categories.HasFlag(ArtistCategories.Producer))
				return 2;

			if (categories.HasFlag(ArtistCategories.Circle) || categories.HasFlag(ArtistCategories.Band))
				return 3;

			return 4;
		}

		// TODO: it'd be better to inject configuration as interface
		public ArtistStringFactory()
			: this(AppConfig.AllowRepeatingProducerAsPerformer) { }

		public ArtistStringFactory(bool allowRepeatingProducerAsPerformer)
		{
			_allowRepeatingProducerAsPerformer = allowRepeatingProducerAsPerformer;
		}

#nullable enable
		public TranslatedStringWithDefault GetArtistString(IEnumerable<IArtistLinkWithRoles> artists, ContentFocus focus)
		{
			ParamIs.NotNull(() => artists);

			var matched = artists.Where(ArtistHelper.IsValidCreditableArtist).ToArray();

			var producers = matched
				.Where(a => ArtistHelper.IsProducerRole(a, focus))
				.OrderBy(a => GetSortOrderForArtistString(a, focus))
				.ToArray();

			var performers = matched
				.Where(a => ArtistHelper.GetCategories(a).HasFlag(ArtistCategories.Vocalist)
					&& (!producers.Contains(a) || _allowRepeatingProducerAsPerformer)
					&& (a.Roles.HasFlag(ArtistRoles.Vocalist) || !a.Roles.HasFlag(ArtistRoles.Chorus)))
				.ToArray();

			const string various = ArtistHelper.VariousArtists;

			if (producers.Length >= 4 || (!producers.Any() && performers.Length >= 4))
				return new TranslatedStringWithDefault(various, various, various, various);

			var performerNames = performers.Select(ArtistHelper.GetTranslatedName);
			var producerNames = producers.Select(ArtistHelper.GetTranslatedName);

			if (producers.Any() && performers.Length > 2 && producers.Length + performers.Length >= 5)
			{
				return TranslatedStringWithDefault.Create(lang => $"{string.Join(", ", producerNames.Select(p => p[lang]))} feat. various");
			}
			else if (producers.Any() && performers.Any())
			{
				return TranslatedStringWithDefault.Create(lang => $"{string.Join(", ", producerNames.Select(p => p[lang]))} feat. {string.Join(", ", performerNames.Select(p => p[lang]))}");
			}
			else
			{
				return TranslatedStringWithDefault.Create(lang => string.Join(", ", (producers.Any() ? producers : performers).Select(a => ArtistHelper.GetTranslatedName(a)[lang])));
			}
		}
#nullable disable
	}
}
