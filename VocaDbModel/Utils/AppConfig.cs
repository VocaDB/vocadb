using System.Configuration;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Utils {

	public static class AppConfig {

		private static ArtistType[] artistTypes;
		private static ArtistRoles[] artistRoles;
		private static SongType[] songTypes;

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
			Domain.Artists.ArtistRoles.Other
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

		private static string Val(string key) {
			return ConfigurationManager.AppSettings[key];
		}

		private static bool Val(string key, bool def) {

			var val = Val(key);
			bool boolVal;
			if (bool.TryParse(val, out boolVal))
				return boolVal;
			else
				return def;
			
		}

		public static bool AllowCustomTracks => Val("AllowCustomTracks", false);

		public static string AllowedCorsOrigins => Val("AllowedCorsOrigins") ?? string.Empty;

		/// <summary>
		/// Allow repeating producer as performer.
		/// This means, if a person appears in both producing and performing (vocalist) roles, they will be duplicated.
		/// For example, Deco feat. Deco.
		/// This was added for UtaiteDB where such behavior is desired.
		/// </summary>
		public static bool AllowRepeatingProducerAsPerformer => Val("AllowRepeatingProducerAsPerformer", false);

		public static ArtistType[] ArtistTypes {
			get {
				
				if (artistTypes == null) {
					var val = Val("ArtistTypes");
					artistTypes = !string.IsNullOrEmpty(val) ? EnumVal<ArtistType>.ParseMultiple(val) : EnumVal<ArtistType>.Values;
				}

				return artistTypes;

			}
		}

		public static ArtistRoles[] ArtistRoles {
			get {
				
				if (artistRoles == null) {
					var val = Val("ArtistRoles");
					artistRoles = !string.IsNullOrEmpty(val) ? EnumVal<ArtistRoles>.ParseMultiple(val) : DefaultValidRoles;
				}

				return artistRoles;

			}
		}

		public static string BilibiliAppKey => Val("BilibiliAppKey");

		public static string BilibiliSecretKey => Val("BilibiliSecretKey");

		public static string BrandedStringsAssembly => Val("BrandedStringsAssembly");

		public static string DbDumpFolder => Val("DbDumpFolder");

		public static string ExternalHelpPath => Val("ExternalHelpPath");

		public static string GAAccountId => Val("GAAccountId");

		public static string GADomain => Val("GADomain");

		public static GlobalLinksSection GetGlobalLinksSection() {
		
			var appLinks = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/appLinks");
			var bigBanners = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/bigBanners");
			var smallBanners = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/smallBanners");
			var socialSites = (LinksSection)ConfigurationManager.GetSection("vocaDb/globalLinks/socialSites");

			return new  GlobalLinksSection { AppLinks = appLinks, BigBanners = bigBanners, SmallBanners  = smallBanners, SocialSites = socialSites };

		}

		/// <summary>
		/// Host address of the main site, contains full path to the web application's root, including hostname.
		/// Could be either HTTP or HTTPS.
		/// For example http://vocadb.net
		/// </summary>
		public static string HostAddress => Val("HostAddress");

		/// <summary>
		/// Host address of the SSL site, used for sensitive actions such as logging in.
		/// For example https://vocadb.net
		/// </summary>
		public static string HostAddressSecure => Val("HostAddressSecure");

		public static string LockdownMessage => Val("LockdownMessage");

		/// <summary>
		/// Preferred artist types when parsing Nico PVs.
		/// For VocaDB Vocaloids are expected instead of human vocalists.
		/// </summary>
		public static ArtistType[] PreferredNicoArtistTypes {
			get {
				var val = Val("PreferredNicoArtistTypes");
				return !string.IsNullOrEmpty(val) ? EnumVal<ArtistType>.ParseMultiple(val) : new ArtistType[0];
			}
		}

		public static string ReCAPTCHAKey => Val("ReCAPTCHAKey");

		public static string ReCAPTCHAPublicKey => Val("ReCAPTCHAPublicKey");

		public static SongType[] SongTypes {
			get {
				
				if (songTypes == null) {
					var val = Val("SongTypes");
					songTypes = !string.IsNullOrEmpty(val) ? EnumVal<SongType>.ParseMultiple(val) : DefaultSongTypes;
				}

				return songTypes;

			}
		}

		public static string SoundCloudClientId => Val("SoundCloudClientId");

		public static string StaticContentPath => Val("StaticContentPath");

		public static string StaticContentHost => Val("StaticContentHost");

		public static string StaticContentHostSSL => Val("StaticContentHostSSL");

		public static string TwitterConsumerKey => Val("TwitterConsumerKey");

		public static string TwitterConsumerSecret => Val("TwitterConsumerSecret");

		public static string YoutubeApiKey => Val("YoutubeApiKey");

	}
}
