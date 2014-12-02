using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;

namespace VocaDb.Web.Controllers
{
    public class SearchController : ControllerBase
    {

		private readonly AlbumService albumService;
		private readonly ArtistService artistService;
        private readonly OtherService services;
		private readonly SongService songService;

		private ActionResult RedirectToAlbum(int id) {
			return RedirectToAction("Details", "Album", new { id });			
		}

		private ActionResult RedirectToArtist(int id) {
			return RedirectToAction("Details", "Artist", new { id });			
		}

		private ActionResult RedirectToSong(int id) {
			return RedirectToAction("Details", "Song", new { id });			
		}

		private ActionResult TryRedirect(string filter, EntryType searchType) {
			
			var matchMode = NameMatchMode.Auto;
			filter = FindHelpers.GetMatchModeAndQueryForSearch(filter, ref matchMode);

			switch (searchType) {
				
				case EntryType.Undefined: {
					var result = services.Find(filter, 1, true);

					if (result.OnlyOneItem) {

						if (result.Albums.TotalCount == 1)
							return RedirectToAlbum(result.Albums.Items[0].Id);

						if (result.Artists.TotalCount == 1)
							return RedirectToArtist(result.Artists.Items[0].Id);

						if (result.Songs.TotalCount == 1)
							return RedirectToSong(result.Songs.Items[0].Id);

						if (result.Tags.TotalCount == 1)
							return RedirectToAction("Details", "Tag", new { id = result.Tags.Items[0].Name });

					}

				}
				break;

				case EntryType.Artist:
					var artist = artistService.FindArtists(new ArtistQueryParams(filter, null, 0, 2, false, false, matchMode, ArtistSortRule.None, false));
					if (artist.Items.Length == 1) {
						return RedirectToArtist(artist.Items[0].Id);
					}
					break;

				case EntryType.Album:
					var album = albumService.Find(new AlbumQueryParams(filter, DiscType.Unknown, 0, 2, false, false, matchMode, AlbumSortRule.None, false));
					if (album.Items.Length == 1) {
						return RedirectToAlbum(album.Items[0].Id);
					}
					break;

				case EntryType.Song:
					var song = songService.Find(new SongQueryParams(filter, null, 0, 2, false, false, matchMode, SongSortRule.None, false, false, null));
					if (song.Items.Length == 1) {
						return RedirectToSong(song.Items[0].Id);
					}
					break;

				default: {
					var action = "Index";
					var controller = searchType.ToString();
					return RedirectToAction(action, controller, new { filter });
				}

			}

			return null;

		}

		public SearchController(OtherService services, ArtistService artistService, AlbumService albumService, SongService songService) {
			this.services = services;
			this.artistService = artistService;
			this.albumService = albumService;
			this.songService = songService;
		}

		public ActionResult Index(string filter, EntryType searchType = EntryType.Undefined, bool allowRedirect = true,
			string tag = null,
			string sort = null, 
			int? artistId = null,
			ArtistType? artistType = null,
			DiscType? discType = null,
			SongType? songType = null,
			bool? onlyWithPVs = null
			) {

			filter = !string.IsNullOrEmpty(filter) ? filter.Trim() : string.Empty;

			if (allowRedirect && !string.IsNullOrEmpty(filter)) {

				var redirectResult = TryRedirect(filter, searchType);

				if (redirectResult != null)
					return redirectResult;

			}

			ViewBag.Query = filter;
			ViewBag.SearchType = searchType != EntryType.Undefined ? searchType.ToString() : "Anything";
			ViewBag.Tag = tag;
			ViewBag.Sort = sort;
			ViewBag.ArtistId = artistId;
			ViewBag.ArtistType = artistType;
			ViewBag.DiscType = discType;
			ViewBag.SongType = songType;
			ViewBag.OnlyWithPVs = onlyWithPVs;

			SetSearchEntryType(searchType);
			return View();

		}

    }
}
