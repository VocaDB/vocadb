using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Model.Service.Search.SongSearch;
using VocaDb.Model.Service.Search.Tags;
using VocaDb.Web.Helpers;
using VocaDb.Web.Models.Search;

namespace VocaDb.Web.Controllers
{
    public class SearchController : ControllerBase
    {

		private readonly AlbumService albumService;
		private readonly ArtistService artistService;
		private readonly EventQueries eventQueries;
		private readonly OtherService services;
		private readonly SongService songService;
		private readonly SongListQueries songListQueries;
		private readonly TagQueries tagQueries;

		private ActionResult RedirectToAlbum(int id) {
			return RedirectToAction("Details", "Album", new { id });			
		}

		private ActionResult RedirectToArtist(int id) {
			return RedirectToAction("Details", "Artist", new { id });			
		}

		private ActionResult RedirectToReleaseEvent(int id) {
			return RedirectToAction("Details", "Event", new { id });
		}

		private ActionResult RedirectToSong(int id) {
			return RedirectToAction("Details", "Song", new { id });			
		}

		private ActionResult RedirectToSongList(int id) {
			return RedirectToAction("Details", "SongList", new { id });
		}

		private ActionResult RedirectToTag(int id, string urlSlug) {
			return RedirectToAction("DetailsById", "Tag", new { id, urlSlug });			
		}

		private ActionResult TryRedirect(string filter, EntryType searchType) {
			
			var textQuery = SearchTextQuery.Create(filter);
			var artistTextQuery = ArtistSearchTextQuery.Create(filter);

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
							return RedirectToTag(result.Tags.Items[0].Id, result.Tags.Items[0].UrlSlug);

					}

				}
				break;

				case EntryType.Artist:
					var artist = artistService.FindArtists(new ArtistQueryParams(artistTextQuery, null, 0, 2, false, false, ArtistSortRule.None, false));
					if (artist.Items.Length == 1) {
						return RedirectToArtist(artist.Items[0].Id);
					}
					break;

				case EntryType.Album:
					var album = albumService.Find(new AlbumQueryParams(textQuery, DiscType.Unknown, 0, 2, false, false, AlbumSortRule.None, false));
					if (album.Items.Length == 1) {
						return RedirectToAlbum(album.Items[0].Id);
					}
					break;

				case EntryType.ReleaseEvent:
					var ev = eventQueries.Find(s => s.Id, textQuery, 0, null, null, 0, 2, false, EventSortRule.Name);
					if (ev.Items.Length == 1) {
						return RedirectToReleaseEvent(ev.Items[0]);
					}
					return RedirectToAction("EventsBySeries", "Event");

				case EntryType.Song:
					var song = songService.Find(new SongQueryParams(textQuery, null, 0, 2, false, false, SongSortRule.None, false, false, null));
					if (song.Items.Length == 1) {
						return RedirectToSong(song.Items[0].Id);
					}
					break;

				case EntryType.SongList:
					var list = songListQueries.Find(s => s.Id, textQuery, null, 0, 2, false, SongListSortRule.Name);
					if (list.Items.Length == 1) {
						return RedirectToSongList(list.Items[0]);
					}
					return RedirectToAction("Featured", "SongList");

				case EntryType.Tag:
					var tags = tagQueries.Find(new TagQueryParams(new CommonSearchParams(textQuery, false, true, true), PagingProperties.FirstPage(2)) { AllowAliases = true },
						TagOptionalFields.None, WebHelper.IsSSL(Request));
					if (tags.Items.Length == 1) {
						return RedirectToTag(tags.Items.First().Id, tags.Items.First().Name);
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

		public SearchController(OtherService services, ArtistService artistService, AlbumService albumService, SongService songService, SongListQueries songListQueries, 
			TagQueries tagQueries, EventQueries eventQueries) {
			this.services = services;
			this.artistService = artistService;
			this.albumService = albumService;
			this.songService = songService;
			this.songListQueries = songListQueries;
			this.tagQueries = tagQueries;
			this.eventQueries = eventQueries;
		}

		public ActionResult Index(SearchIndexViewModel viewModel) {

			if (viewModel == null)
				viewModel = new SearchIndexViewModel();

			var filter = viewModel.Filter;
			filter = !string.IsNullOrEmpty(filter) ? filter.Trim() : string.Empty;

			if (viewModel.AllowRedirect && !string.IsNullOrEmpty(filter)) {

				var redirectResult = TryRedirect(filter, viewModel.SearchType);

				if (redirectResult != null)
					return redirectResult;

			}

			if (!string.IsNullOrEmpty(viewModel.Tag)) {
				viewModel.TagId = new[] { tagQueries.GetTagIdByName(viewModel.Tag) };
			}

			viewModel.Filter = filter;

			SetSearchEntryType(viewModel.SearchType);

			return View("Index", viewModel);

		}

		public ActionResult Radio() {
			
			return Index(new SearchIndexViewModel(EntryType.Song) { MinScore = 1, Sort = "AdditionDate", ViewMode = "PlayList", Autoplay = true, Shuffle = true });

		}

    }
}
