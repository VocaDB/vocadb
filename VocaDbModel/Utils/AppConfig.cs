#nullable disable

using System.Configuration;
using NHibernate.Linq.Functions;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Utils
{
	public static class AppConfig
	{
#nullable enable
		private static DiscType[]? s_albumTypes;
		private static ArtistType[]? s_artistTypes;
		private static ArtistRoles[]? s_artistRoles;
		private static SongType[]? s_songTypes;
#nullable disable

		/// <summary>
		/// List of roles that can be assigned to artist added to songs and albums.
		/// The list should be in the correct order.
		/// The Default role is excluded because it's not a valid selection.
		/// </summary>
		private static readonly ArtistRoles[] DefaultValidRoles = {
			Domain.Artists.ArtistRoles.Animator,
			Domain.Artists.ArtistRoles.Arranger,
			Domain.Artists.ArtistRoles.Composer,
			Domain.Artists.ArtistRoles.Distributor,
			Domain.Artists.ArtistRoles.Illustrator,
			Domain.Artists.ArtistRoles.Instrumentalist,
			Domain.Artists.ArtistRoles.Lyricist,
			Domain.Artists.ArtistRoles.Mastering,
			Domain.Artists.ArtistRoles.Mixer,
			Domain.Artists.ArtistRoles.Publisher,
			Domain.Artists.ArtistRoles.Vocalist,
			Domain.Artists.ArtistRoles.VoiceManipulator,
			Domain.Artists.ArtistRoles.VocalDataProvider,
			Domain.Artists.ArtistRoles.Other
		};

		private static readonly DiscType[] DefaultDiscTypes = {
			DiscType.Unknown,
			DiscType.Album,
			DiscType.Single,
			DiscType.EP,
			DiscType.SplitAlbum,
			DiscType.Compilation,
			DiscType.Video,
			DiscType.Artbook,
			DiscType.Other
		};

		private static readonly SongType[] DefaultSongTypes = {
			SongType.Unspecified,
			SongType.Original,
			SongType.Remaster,
			SongType.Remix,
			SongType.Cover,
			SongType.Instrumental,
			SongType.Mashup,
			SongType.MusicPV,
			SongType.DramaPV,
			SongType.Other
		};

#nullable enable
		private static string? Val(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}
#nullable disable

		private static bool Val(string key, bool def)
		{
			var val = Val(key);
			return (bool.TryParse(val, out var boolVal)) ? boolVal : def;
		}

		private static int Val(string key, int def)
		{
			var val = Val(key);
			return (int.TryParse(val, out var boolVal)) ? boolVal : def;
		}

		public static bool AllowCustomArtistName => Val("AllowCustomArtistName", false);

#nullable enable
		public static DiscType[] AlbumTypes
		{
			get
			{
				if (s_albumTypes == null)
				{
					var val = Val("AlbumTypes");
					s_albumTypes = !string.IsNullOrEmpty(val) ? EnumVal<DiscType>.ParseMultiple(val) : DefaultDiscTypes;
				}

				return s_albumTypes;
			}
		}
#nullable disable

		public static bool AllowCustomTracks => Val("AllowCustomTracks", false);

		public static string AllowedCorsOrigins => Val("AllowedCorsOrigins") ?? string.Empty;

		/// <summary>
		/// Allow repeating producer as performer.
		/// This means, if a person appears in both producing and performing (vocalist) roles, they will be duplicated.
		/// For example, Deco feat. Deco.
		/// This was added for UtaiteDB where such behavior is desired.
		/// </summary>
		public static bool AllowRepeatingProducerAsPerformer => Val("AllowRepeatingProducerAsPerformer", false);

#nullable enable
		public static ArtistType[] ArtistTypes
		{
			get
			{
				if (s_artistTypes == null)
				{
					var val = Val("ArtistTypes");
					s_artistTypes = !string.IsNullOrEmpty(val) ? EnumVal<ArtistType>.ParseMultiple(val) : EnumVal<ArtistType>.Values;
				}

				return s_artistTypes;
			}
		}

		public static ArtistRoles[] ArtistRoles
		{
			get
			{
				if (s_artistRoles == null)
				{
					var val = Val("ArtistRoles");
					s_artistRoles = !string.IsNullOrEmpty(val) ? EnumVal<ArtistRoles>.ParseMultiple(val) : DefaultValidRoles;
				}

				return s_artistRoles;
			}
		}
#nullable disable

		public static string BilibiliAppKey => Val("BilibiliAppKey");

		public static string BilibiliSecretKey => Val("BilibiliSecretKey");

		public static string BrandedStringsAssembly => Val("BrandedStringsAssembly");

		public static string DbDumpFolder => Val("DbDumpFolder");

		/// <summary>
		/// Enable inheriting artists for certain situations.
		/// Currently only Subject artist type is inherited.
		/// </summary>
		public static bool EnableArtistInheritance => Val(nameof(EnableArtistInheritance), false);

		public static string ExternalHelpPath => Val(nameof(ExternalHelpPath));

		public static int FilteredArtistId => Val("FilteredArtistId", 0);

		public static string GAAccountId => Val(nameof(GAAccountId));

		public static string GADomain => Val(nameof(GADomain));

		public static GlobalLinksSection GetGlobalLinksSection()
		{
			var appLinks = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/appLinks");
			var bigBanners = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/bigBanners");
			var smallBanners = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/smallBanners");
			var socialSites = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/socialSites");

			return new GlobalLinksSection { AppLinks = appLinks, BigBanners = bigBanners, SmallBanners = smallBanners, SocialSites = socialSites };
		}

		public static SlogansSection GetSlogansSection()
		{
			return (SlogansSection)ConfigurationManager.GetSection("vocaDb/slogans");
		}

#nullable enable
		/// <summary>
		/// Host address of the main site, contains full path to the web application's root, including hostname.
		/// For example https://vocadb.net
		/// </summary>
		public static string HostAddress => Val("HostAddress");

		public static string? LockdownMessage => Val("LockdownMessage");
#nullable disable

		/// <summary>
		/// Preferred artist types when parsing Nico PVs.
		/// For VocaDB Vocaloids are expected instead of human vocalists.
		/// </summary>
		public static ArtistType[] PreferredNicoArtistTypes
		{
			get
			{
				var val = Val("PreferredNicoArtistTypes");
				return !string.IsNullOrEmpty(val) ? EnumVal<ArtistType>.ParseMultiple(val) : new ArtistType[0];
			}
		}

		public static string ReCAPTCHAKey => Val("ReCAPTCHAKey");

		public static string ReCAPTCHAPublicKey => Val("ReCAPTCHAPublicKey");

		public static SiteSettingsSection SiteSettings => new VdbConfigManager().SiteSettings;

#nullable enable
		public static SongType[] SongTypes
		{
			get
			{
				if (s_songTypes == null)
				{
					var val = Val("SongTypes");
					s_songTypes = !string.IsNullOrEmpty(val) ? EnumVal<SongType>.ParseMultiple(val) : DefaultSongTypes;
				}

				return s_songTypes;
			}
		}
#nullable disable

		public static string SoundCloudClientId => Val("SoundCloudClientId");

#nullable enable
		public static string StaticContentPath => Val("StaticContentPath");

		public static string StaticContentHost => Val("StaticContentHost");
#nullable disable

		public static string TwitterConsumerKey => Val("TwitterConsumerKey");

		public static string TwitterConsumerSecret => Val("TwitterConsumerSecret");

		public static string YoutubeApiKey => Val("YoutubeApiKey");

		public static string VimeoApiKey => Val("VimeoApiKey");
	}
}
