using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Helpers {

	public static class ArtistHelper {

		public const string VariousArtists = "Various artists";

		public static TranslatedString GetTranslatedName(IArtistWithSupport link) {

			return (link.Artist != null ? link.Artist.TranslatedName : TranslatedString.Create(link.Name));

		}

		public static bool IsProducerRole(IArtistWithSupport link, bool isAnimation) {

			return IsProducerRole(GetCategories(link), isAnimation);

		}

		private static bool IsProducerRole(ArtistCategories categories, bool isAnimation) {

			return (categories.HasFlag(ArtistCategories.Producer) 
				|| categories.HasFlag(ArtistCategories.Circle) 
				|| categories.HasFlag(ArtistCategories.Band) 
				|| (isAnimation && categories.HasFlag(ArtistCategories.Animator)));

		}

		public static bool IsValidCreditableArtist(IArtistWithSupport artist) {

			if (artist.IsSupport)
				return false;

			var cat = GetCategories(artist);

			return (cat != ArtistCategories.Nothing && cat != ArtistCategories.Label);

		}

		public static readonly Dictionary<ArtistType, ArtistCategories> CategoriesForTypes = new Dictionary<ArtistType, ArtistCategories> {
			{ ArtistType.Animator, ArtistCategories.Animator },
			{ ArtistType.Circle, ArtistCategories.Circle },
			{ ArtistType.Band, ArtistCategories.Band },
			{ ArtistType.Illustrator, ArtistCategories.Other },
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
		};

		/// <summary>
		/// The roles of these artists can be customized
		/// </summary>
		public static readonly ArtistType[] CustomizableTypes = {
			ArtistType.Animator, ArtistType.OtherGroup, ArtistType.OtherIndividual, 
			ArtistType.OtherVocalist, ArtistType.Producer, ArtistType.Illustrator, ArtistType.Lyricist, 
			ArtistType.Utaite, ArtistType.Band, ArtistType.Unknown
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
		public static readonly ArtistType[] SongArtistTypes = {
			ArtistType.Unknown, ArtistType.OtherGroup, ArtistType.OtherVocalist,
			ArtistType.Producer, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.Vocaloid, ArtistType.Animator, ArtistType.Illustrator,
			ArtistType.Lyricist, ArtistType.OtherIndividual
		};

		public static readonly ArtistType[] VocalistTypes = {
			ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.OtherVocalist, ArtistType.OtherVoiceSynthesizer, ArtistType.Utaite
		};

		public static TranslatedStringWithDefault GetArtistString(IEnumerable<IArtistWithSupport> artists, bool isAnimation) {

			return new ArtistStringFactory().GetArtistString(artists, isAnimation);

		}

		/// <summary>
		/// Returns the canonized artist name. Basically this means removing any P suffixes.
		/// </summary>
		/// <param name="name">Artist name that may be canonized or not. Can be null or empty, in which case nothing is done.</param>
		/// <returns>Canonized artist name.</returns>
		public static string GetCanonizedName(string name) {

			if (string.IsNullOrEmpty(name))
				return name;

			var queryWithoutP = (name.EndsWith("-P", StringComparison.InvariantCultureIgnoreCase) ? name.Substring(0, name.Length - 2) : name);
			queryWithoutP = (queryWithoutP.EndsWith("P", StringComparison.InvariantCultureIgnoreCase) ? queryWithoutP.Substring(0, queryWithoutP.Length - 1) : queryWithoutP);

			return queryWithoutP;

		}

		public static ArtistCategories GetCategories(IArtistWithSupport artist) {

			ParamIs.NotNull(() => artist);

			return GetCategories(artist.Artist != null ? artist.Artist.ArtistType : ArtistType.Unknown, artist.Roles);

		}

		public static ArtistCategories GetCategories(ArtistType type, ArtistRoles roles) {

			if (roles == ArtistRoles.Default || !IsCustomizable(type)) {

				return CategoriesForTypes[type];

			} else {

				var cat = ArtistCategories.Nothing;

				if (roles.HasFlag(ArtistRoles.Vocalist) || roles.HasFlag(ArtistRoles.Chorus))
					cat |= ArtistCategories.Vocalist;

				if (roles.HasFlag(ArtistRoles.Arranger) || roles.HasFlag(ArtistRoles.Composer) || roles.HasFlag(ArtistRoles.VoiceManipulator))
					cat |= ArtistCategories.Producer;

				if (roles.HasFlag(ArtistRoles.Distributor) || roles.HasFlag(ArtistRoles.Publisher))
					cat |= ArtistCategories.Circle;

				if (roles.HasFlag(ArtistRoles.Animator))
					cat |= ArtistCategories.Animator;

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
		public static ArtistType[] GetArtistTypesFromFlags(ArtistTypes flags) {
			
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
		/// <param name="isAnimation">Whether animation producers should be considered as well.</param>
		/// <returns>The main circle, or null if there is none.</returns>
		public static Artist GetMainCircle(IList<IArtistWithSupport> artists, bool isAnimation) {

			var producers = GetProducers(artists.Where(a => !a.IsSupport), isAnimation).ToArray();

			// Find the circle in which all the producers belong to
			var circle = artists.FirstOrDefault(a => a.Artist != null 
				&& GetCategories(a).HasFlag(ArtistCategories.Circle)
				&& producers.All(p => p.Artist != null && (p.Artist.Equals(a.Artist) || p.Artist.HasGroup(a.Artist))));

			return circle != null ? circle.Artist : null;

		}

		public static ArtistRoles GetOtherArtistRoles(ArtistType artistType) {

			switch (artistType) {
				case ArtistType.Illustrator:
					return ArtistRoles.Illustrator;

				case ArtistType.Lyricist:
					return ArtistRoles.Lyricist;

				default:
					return ArtistRoles.Default;
			}

		}

		public static IEnumerable<IArtistWithSupport> GetProducers(IEnumerable<IArtistWithSupport> artists, bool isAnimation) {
			return artists.Where(a => IsProducerRole(a, isAnimation));
		}

		public static string[] GetProducerNames(IEnumerable<IArtistWithSupport> artists, bool isAnimation, ContentLanguagePreference languagePreference) {

			var matched = artists.Where(IsValidCreditableArtist).ToArray();
			var producers = matched.Where(a => IsProducerRole(a, isAnimation)).ToArray();
			var names = producers.Select(p => GetTranslatedName(p).GetBestMatch(languagePreference)).ToArray();

			return names;

		}

		public static string[] GetVocalistNames(IEnumerable<IArtistWithSupport> artists, ContentLanguagePreference languagePreference) {

			var matched = artists.Where(IsValidCreditableArtist).ToArray();
			var vocalists = GetVocalists(matched);
			var names = vocalists.Select(p => GetTranslatedName(p).GetBestMatch(languagePreference)).ToArray();

			return names;

		}

		public static IEnumerable<IArtistWithSupport> GetVocalists(IList<IArtistWithSupport> artists) {
			return artists.Where(a => GetCategories(a).HasFlag(ArtistCategories.Vocalist));
		}

		public static bool IsCustomizable(ArtistType at) {

			return CustomizableTypes.Contains(at);

		}

	}

}
