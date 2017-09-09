using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using VocaDb.SiteMapGenerator.Sitemap;
using VocaDb.SiteMapGenerator.VocaDb;

namespace VocaDb.SiteMapGenerator {

	class Program {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private static async Task GenerateSitemap() {
			
			var config = new Config();
			var client = new VocaDbClient(config.SiteRootUrl);

			log.Info("Getting entries from VocaDB");

			var artists = await client.GetArtists();
			var albums = await client.GetAlbums();
			var events = await client.GetEvents();
			var songs = await client.GetSongs();
			var tags = await client.GetTags();

			log.Info("Generating sitemap");

			var generator = new SitemapGenerator(config.SiteRootUrl, config.SitemapRootUrl);
			generator.Generate(config.OutFolder, new Dictionary<EntryType, IEnumerable<EntryReference>> {
				{ EntryType.Artist, artists.Select(e => new EntryReference(e)) },
				{ EntryType.Album, albums.Select(e => new EntryReference(e)) },
				{ EntryType.ReleaseEvent, events.Items.Select(e => new EntryReference(e.Id, e.UrlSlug)) },
				{ EntryType.Song, songs.Select(e => new EntryReference(e)) },
				{ EntryType.Tag, tags.Items.Select(e => new EntryReference(e.Id, e.UrlSlug)) },
			});

		}

		static void Main(string[] args) {

			Task.WaitAll(GenerateSitemap());

		}
	}
}
