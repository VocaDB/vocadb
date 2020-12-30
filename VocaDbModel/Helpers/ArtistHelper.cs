#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Helpers
{
	public static class ArtistHelper
	{
		public const string VariousArtists = "Various artists";

		public static TranslatedString GetTranslatedName(IArtistLinkWithRoles link)
		{
			return (link.Artist != null && string.IsNullOrEmpty(link.Name) ? link.Artist.TranslatedName : TranslatedString.Create(link.Name));
		}

		public static bool IsProducerRole(IArtistLinkWithRoles link, ContentFocus focus)
		{
			return IsProducerRole(GetCategories(link), focus);
		}

		private static bool IsProducerRole(ArtistCategories categories, ContentFocus focus)
		{
			return (categories.HasFlag(ArtistCategories.Producer)
				|| categories.HasFlag(ArtistCategories.Circle)
				|| categories.HasFlag(ArtistCategories.Band)
				|| (focus == ContentFocus.Video && categories.HasFlag(ArtistCategories.Animator))
				|| (focus == ContentFocus.Illustration && categories.HasFlag(ArtistCategories.Illustrator)));
		}

		/// <summary>
		/// Whether artist should appear in artist string.
		/// </summary>
		public static bool IsValidCreditableArtist(IArtistLinkWithRoles artist)
		{
			if (artist.IsSupport)
				return false;

			var cat = GetCategories(artist);

			return (cat != ArtistCategories.Nothing && cat != ArtistCategories.Label);
		}

		public static readonly Dictionary<ArtistType, ArtistCategories> CategoriesForTypes = new()
		{
			{ ArtistType.Animator, ArtistCategories.Animator },
			{ ArtistType.Character, ArtistCategories.Subject },
			{ ArtistType.Circle, ArtistCategories.Circle },
			{ ArtistType.Band, ArtistCategories.Band },
			{ ArtistType.Illustrator, ArtistCategories.Illustrator },
			{ ArtistType.Label, ArtistCategories.Label },
			{ ArtistType.Lyricist, ArtistCategories.Other },
			{ ArtistType.OtherGroup, ArtistCategories.Circle },
			{ ArtistType.OtherIndividual, ArtistCategories.Other },
			{ ArtistType.OtherVocalist, ArtistCategories.Vocalist },
			{ ArtistType.OtherVoiceSynthesizer, ArtistCategories.Vocalist },
			{ ArtistType.Producer, ArtistCategories.Producer },
			{ ArtistType.Unknown, ArtistCategories.Other },
			{ ArtistType.Utaite, ArtistCategories.Vocalist },
			{ ArtistType.UTAU, ArtistCategories.Vocalist },
			{ ArtistType.CeVIO, ArtistCategories.Vocalist },
			{ ArtistType.Vocaloid, ArtistCategories.Vocalist },
			{ ArtistType.Vocalist, ArtistCategories.Vocalist },
			{ ArtistType.SynthesizerV, ArtistCategories.Vocalist },
		};

		/// <summary>
		/// The roles of these artists can be customized
		/// </summary>
		public static readonly ArtistType[] CustomizableTypes =
		{
			ArtistType.Animator, ArtistType.OtherGroup, ArtistType.OtherIndividual,
			ArtistType.OtherVocalist, ArtistType.Producer, ArtistType.Illustrator, ArtistType.Lyricist,
			ArtistType.Utaite, ArtistType.Band, ArtistType.Vocalist, ArtistType.Unknown
		};

		public static readonly ArtistType[] GroupTypes =
		{
			ArtistType.Label, ArtistType.Circle, ArtistType.OtherGroup, ArtistType.Band
		};

		/// <summary>
		/// List of roles that can be assigned to artist added to songs and albums.
		/// The list should be in the correct order.
		/// The Default role is excluded because it's not a valid selection.
		/// </summary>
		public static ArtistRoles[] ValidRoles => AppConfig.ArtistRoles;

		//public static readonly ArtistType[] ProducerTypes = new[] {
		//	ArtistType.Producer, ArtistType.Circle, ArtistType.OtherGroup, ArtistType.Animator
		//};

		/// <summary>
		/// Artists allowed for a song.
		/// </summary>
		public static readonly ArtistType[] SongArtistTypes =
		{
			ArtistType.Unknown, ArtistType.OtherGroup, ArtistType.OtherVocalist,
			ArtistType.Producer, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.Vocaloid, ArtistType.Animator, ArtistType.Illustrator,
			ArtistType.Lyricist, ArtistType.OtherIndividual, ArtistType.Character,
			ArtistType.SynthesizerV,
		};

		public static readonly ArtistType[] VocalistTypes =
		{
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.OtherVocalist,
			ArtistType.OtherVoiceSynthesizer, ArtistType.Utaite, ArtistType.Vocalist,
			ArtistType.SynthesizerV,
		};

		/// <summary>
		/// Voice synthesizer artist types, including "other voice synthesizer".
		/// </summary>
		public static readonly ArtistType[] VoiceSynthesizerTypes =
		{
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.OtherVoiceSynthesizer,
			ArtistType.SynthesizerV,
		};

		/// <summary>
		/// Whether artist type can have artist links of a specified type and direction.
		/// </summary>
		/// <param name="artistType">Artist type to be tested.</param>
		/// <param name="linkType">Link type.</param>
		/// <param name="direction">Link direction.</param>
		/// <returns>True if the artist type can have artist links, otherwise false.</returns>
		public static bool CanHaveRelatedArtists(ArtistType artistType, ArtistLinkType linkType, LinkDirection direction)
		{
			if (artistType == ArtistType.Unknown)
				return true;

			if (linkType == ArtistLinkType.Group)
			{
				return direction == LinkDirection.ManyToOne || GroupTypes.Contains(artistType);
			}

			return direction == LinkDirection.ManyToOne ? VocalistTypes.Contains(artistType) : !VocalistTypes.Contains(artistType) || artistType == ArtistType.OtherVocalist;
		}

		public static TranslatedStringWithDefault GetArtistString(IEnumerable<IArtistLinkWithRoles> artists, ContentFocus focus)
		{
			return new ArtistStringFactory().GetArtistString(artists, focus);
		}

		/// <summary>
		/// Returns the canonized artist name. Basically this means removing any P suffixes.
		/// </summary>
		/// <param name="name">Artist name that may be canonized or not. Can be null or empty, in which case nothing is done.</param>
		/// <returns>Canonized artist name.</returns>
		public static string GetCanonizedName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return name;

			var queryWithoutP = (name.EndsWith("-P", StringComparison.InvariantCultureIgnoreCase) ? name.Substring(0, name.Length - 2) : name);
			queryWithoutP = (queryWithoutP.EndsWith("P", StringComparison.InvariantCultureIgnoreCase) ? queryWithoutP.Substring(0, queryWithoutP.Length - 1) : queryWithoutP);

			return queryWithoutP;
		}

		public static ArtistCategories GetCategories(IArtistLinkWithRoles artist)
		{
			ParamIs.NotNull(() => artist);

			return GetCategories(artist.Artist != null ? artist.Artist.ArtistType : ArtistType.Unknown, artist.Roles);
		}

		public static ArtistCategories GetCategories(ArtistType type, ArtistRoles roles)
		{
			if (roles == ArtistRoles.Default || !IsCustomizable(type))
			{
				return CategoriesForTypes[type];
			}
			else
			{
				var cat = ArtistCategories.Nothing;

				if (roles.HasFlag(ArtistRoles.Vocalist) || roles.HasFlag(ArtistRoles.Chorus))
					cat |= ArtistCategories.Vocalist;

				if (roles.HasFlag(ArtistRoles.Arranger) || roles.HasFlag(ArtistRoles.Composer) || roles.HasFlag(ArtistRoles.VoiceManipulator))
					cat |= ArtistCategories.Producer;

				if (roles.HasFlag(ArtistRoles.Distributor) || roles.HasFlag(ArtistRoles.Publisher))
					cat |= ArtistCategories.Circle;

				if (roles.HasFlag(ArtistRoles.Animator))
					cat |= ArtistCategories.Animator;

				if (roles.HasFlag(ArtistRoles.Illustrator))
					cat |= ArtistCategories.Illustrator;

				//if (roles.HasFlag(ArtistRoles.Illustrator) || roles.HasFlag(ArtistRoles.Lyricist) || roles.HasFlag(ArtistRoles.Mastering))
				//	cat |= ArtistCategories.Other;

				if (cat == ArtistCategories.Nothing)
					cat = ArtistCategories.Other;

				return cat;
			}
		}

		/// <summary>
		/// Gets a list of individual values from bitarray.
		/// <see cref="ArtistTypes.Unknown"/> is skipped.
		/// </summary>
		/// <param name="flags">Bitarray of artist types.</param>
		/// <returns>Individual artist types.</returns>
		public static ArtistType[] GetArtistTypesFromFlags(ArtistTypes flags)
		{
			return EnumVal<ArtistTypes>
				.GetIndividualValues(flags)
				.Where(v => v != ArtistTypes.Unknown)
				.Select(a => EnumVal<ArtistType>.Parse(a.ToString()))
				.ToArray();
		}

		/// <summary>
		/// Gets the main circle from a group of artists, or null if there is no main circle.
		/// Here main circle is defined as the circle in which all the producers of the album belong to.
		/// </summary>
		/// <param name="artists">List of artists. Cannot be null.</param>
		/// <param name="focus">Determines types of producers to consider.</param>
		/// <returns>The main circle, or null if there is none.</returns>
		public static Artist GetMainCircle(IList<IArtistLinkWithRoles> artists, ContentFocus focus)
		{
			var producers = GetProducers(artists.Where(a => !a.IsSupport), focus).ToArray();

			// Find the circle in which all the producers belong to
			var circle = artists.FirstOrDefault(a => a.Artist != null
				&& GetCategories(a).HasFlag(ArtistCategories.Circle)
				&& producers.All(p => p.Artist != null && (p.Artist.Equals(a.Artist) || p.Artist.HasGroup(a.Artist))));

			return circle != null ? circle.Artist : null;
		}

		public static ArtistRoles GetOtherArtistRoles(ArtistType artistType) => artistType switch
		{
			ArtistType.Illustrator => ArtistRoles.Illustrator,
			ArtistType.Lyricist => ArtistRoles.Lyricist,
			_ => ArtistRoles.Default,
		};

		public static IEnumerable<IArtistLinkWithRoles> GetProducers(IEnumerable<IArtistLinkWithRoles> artists, ContentFocus focus)
		{
			return artists.Where(a => IsProducerRole(a, focus));
		}

		public static string[] GetProducerNames(IEnumerable<IArtistLinkWithRoles> artists, ContentFocus focus, ContentLanguagePreference languagePreference)
		{
			var matched = artists.Where(IsValidCreditableArtist).ToArray();
			var producers = matched.Where(a => IsProducerRole(a, focus)).ToArray();
			var names = producers.Select(p => GetTranslatedName(p).GetBestMatch(languagePreference)).ToArray();

			return names;
		}

		public static string[] GetVocalistNames(IEnumerable<IArtistLinkWithRoles> artists, ContentLanguagePreference languagePreference)
		{
			var matched = artists.Where(IsValidCreditableArtist).ToArray();
			var vocalists = GetVocalists(matched);
			var names = vocalists.Select(p => GetTranslatedName(p).GetBestMatch(languagePreference)).ToArray();

			return names;
		}

		public static IEnumerable<IArtistLinkWithRoles> GetVocalists(IList<IArtistLinkWithRoles> artists)
		{
			return artists.Where(a => GetCategories(a).HasFlag(ArtistCategories.Vocalist));
		}

		public static bool IsCustomizable(ArtistType at)
		{
			return CustomizableTypes.Contains(at);
		}

		public static bool IsVoiceSynthesizer(ArtistType artistType)
		{
			return VoiceSynthesizerTypes.Contains(artistType);
		}
	}
}
