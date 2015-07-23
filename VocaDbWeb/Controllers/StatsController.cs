using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.Repositories;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers {

	internal static class StatsExtensions {
			
		public static IEnumerable<TResult> CumulativeSelect<TSource, TResult>(this IEnumerable<TSource> sequence, Func<TSource, TResult, TResult> func)
		{
			var previous = default(TResult);
			foreach(var item in sequence)
			{
				var res = func(item, previous);
				previous = res;
				yield return res;
			}        
		}

	}

	public class StatsController : ControllerBase {

		private const int clientCacheDurationSec = 86400;

		private T GetCachedReport<T>() where T : class {

			var name = ControllerContext.RouteData.Values["action"];
			var item = context.Cache["report_" + name];

			if (item == null)
				return null;

			return (T)item;

		}

		private void SaveCachedReport<T>(T data) where T : class {
			
			var name = ControllerContext.RouteData.Values["action"];
			context.Cache.Add("report_" + name, data, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1), CacheItemPriority.Default, null);

		}

		class LocalizedValue {

			public int EntryId { get; set; }

			public TranslatedString Name { get; set; }

			public int Value { get; set; }

		}

		private ActionResult DateLineChartWithAverage(string title, string pointsTitle, string yAxisTitle, ICollection<Tuple<DateTime, int>> points,
			bool average = true) {
			
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
			Response.Cache.SetSlidingExpiration(true);

			return Json(HighchartsHelper.DateLineChartWithAverage(title, pointsTitle, yAxisTitle, points, average));

		}

		private ActionResult SimpleBarChart(string title, string seriesName, IList<string> categories, IList<int> data) {
			
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
			Response.Cache.SetSlidingExpiration(true);

			return Json(new {
				chart = new {
					type = "bar",
					height = 600
				},
				title = new {
					text = title
				},
				xAxis = new {
					categories,
					title = new {
						text = (string)null
					}
				},
				yAxis = new {
					title = new {
						text = seriesName
					}
				},
				plotOptions = new {
					bar = new {
						dataLabels = new {
							enabled = true
						}
					}
				},
				legend = new {
					enabled = false
				},
				series = new Object[] {
					new {
						name = seriesName,
						data						
					}
				}
				
			});

		}

		private ActionResult SimpleBarChart<T>(Func<IQueryable<T>, IQueryable<LocalizedValue>> func, string title, string seriesName) {

			var values = GetTopValues(func);

			var categories = values.Select(p => p.Name[permissionContext.LanguagePreference]).ToArray();
			var data = values.Select(p => p.Value).ToArray();

			return SimpleBarChart(title, seriesName, categories, data);

		}

		private ActionResult SimplePieChart(string title, string seriesName, ICollection<Tuple<string, int>> points) {
			
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
			Response.Cache.SetSlidingExpiration(true);

			return Json(HighchartsHelper.SimplePieChart(title, seriesName, points, false));

		}

		private ICollection<Tuple<string, int>> GetGenreTagUsages<T>() where T : TagUsage {
			
			return userRepository.HandleQuery(ctx => {
				
				var genres = ctx.OfType<T>()
					.Query()
					.Where(u => u.Tag.AliasedTo == null && u.Tag.Parent == null && u.Tag.CategoryName == Tag.CommonCategory_Genres)
					.GroupBy(s => s.Tag.Name)
					.Select(g => new {
						TagName = g.Key,
						Count = g.Count()
					})
					.OrderByDescending(g => g.Count)
					.ToArray();

				var mainGenres = genres.Take(10).ToArray();
				var otherCount = genres.Skip(10).Sum(g => g.Count);
				var points = mainGenres.Concat(new[] { new {
					TagName = "Other genres", 
					Count = otherCount
				} }).Select(g => Tuple.Create(g.TagName, g.Count)).ToArray();

				return points;

			});

		}

		private LocalizedValue[] GetTopValues<T>(Func<IQueryable<T>, IQueryable<LocalizedValue>> func) {
			
			var cached = GetCachedReport<LocalizedValue[]>();

			if (cached != null)
				return cached;

			var data = userRepository.HandleQuery(ctx => {
				
				return func(ctx.OfType<T>().Query())
					.OrderByDescending(a => a.Value)
					.Take(25)
					.ToArray();

			});

			SaveCachedReport(data);

			return data;

		}

		private readonly HttpContextBase context;
		private readonly IUserPermissionContext permissionContext;
		private readonly IRepository repository;
		private readonly IUserRepository userRepository;

		public StatsController(IUserRepository userRepository, IRepository repository, IUserPermissionContext permissionContext, HttpContextBase context) {
			this.userRepository = userRepository;
			this.repository = repository;
			this.permissionContext = permissionContext;
			this.context = context;
		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult AlbumsPerGenre() {
			
			var points = GetGenreTagUsages<AlbumTagUsage>();
			return SimplePieChart("Albums per genre", "Albums", points);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult AlbumsPerMonth() {
			
			var now = DateTime.Now;

			var values = userRepository.HandleQuery(ctx => {

				return ctx.OfType<Album>().Query()
					.Where(a => !a.Deleted 
						&& a.OriginalRelease.ReleaseDate.Year != null 
						&& a.OriginalRelease.ReleaseDate.Month != null 
						&& (a.OriginalRelease.ReleaseDate.Year < now.Year || (a.OriginalRelease.ReleaseDate.Year == now.Year && a.OriginalRelease.ReleaseDate.Month <= now.Month))
						&& a.AllArtists.Any(r => 
							r.Artist.ArtistType == ArtistType.Vocaloid 
							|| r.Artist.ArtistType == ArtistType.UTAU
							|| r.Artist.ArtistType == ArtistType.CeVIO
							|| r.Artist.ArtistType == ArtistType.OtherVoiceSynthesizer
							|| r.Artist.ArtistType == ArtistType.Utaite))
					.OrderBy(a => a.OriginalRelease.ReleaseDate.Year)
					.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
					.GroupBy(a => new {
						Year = a.OriginalRelease.ReleaseDate.Year, 
						Month = a.OriginalRelease.ReleaseDate.Month
					})
					.Select(a => new {
						a.Key.Year,
						a.Key.Month,
						Count = a.Count()
					})
					.ToArray();

			});

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year.Value, v.Month.Value, 1), v.Count)).ToArray();

			return DateLineChartWithAverage("Releases by month", "Albums", "Albums released", points);

		}

		public ActionResult AlbumsPerProducer() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllAlbums.Count(s => !s.IsSupport && !s.Album.Deleted && s.Album.DiscType != DiscType.Compilation),
						EntryId = a.Id
					}), "Albums by producer", "Songs");

		}

		public ActionResult AlbumsPerVocaloid() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => 
						a.ArtistType == ArtistType.Vocaloid || 
						a.ArtistType == ArtistType.UTAU ||
                        a.ArtistType == ArtistType.CeVIO ||
                        a.ArtistType == ArtistType.Utaite)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllAlbums.Count(s => !s.IsSupport && !s.Album.Deleted)
					}), "Albums by Vocaloid/UTAU", "Songs");

		}

		class DateWithCount {
			public int Year { get; set; }
			public int Month { get; set; }
			public int Day { get; set; }
			public int Count { get; set; }
		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult CumulativeAlbums() {
			
			var values = userRepository.HandleQuery(ctx => {

				return ctx.OfType<Album>().Query()
					.OrderBy(a => a.CreateDate.Year)
					.ThenBy(a => a.CreateDate.Month)
					.ThenBy(a => a.CreateDate.Day)
					.GroupBy(a => new {
						Year = a.CreateDate.Year, 
						Month = a.CreateDate.Month,
						Day = a.CreateDate.Day
					})
					.Select(a => new DateWithCount {
						Year = a.Key.Year,
						Month = a.Key.Month,
						Day = a.Key.Day,
						Count = a.Count()
					})
					.ToArray();

			});

			var points = values.CumulativeSelect<DateWithCount, Tuple<DateTime, int>>((v, previous) => {
				return Tuple.Create(new DateTime(v.Year, v.Month, v.Day), (previous != null ? previous.Item2 : 0) + v.Count);
			}).ToArray();

			return DateLineChartWithAverage("Cumulative albums per day", "Albums", "Number of albums", points, false);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult EditsPerDay() {
			
			var points = new DataAccess.ActivityEntryQueries(repository).GetEditsPerDay(null);

			return DateLineChartWithAverage("Edits per day", "Edits", "Number of edits", points);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult EditsPerUser(DateTime? cutoff) {
			
			return SimpleBarChart<ActivityEntry>(q => { 
			
				if (cutoff.HasValue)
					q = q.Where(a => a.CreateDate >= cutoff);
	
				return q
					.GroupBy(a => a.Author.Name)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = ContentLanguageSelection.Japanese,
 							Japanese = a.Key
						},
						Value = a.Count(),
					});

			}, "Edits per user", "User");

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult EditsPerUserLastYear() {
			
			var cutoff = DateTime.Now - TimeSpan.FromDays(365);

			return EditsPerUser(cutoff);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult SongsAddedPerDay() {
			
			var values = repository.HandleQuery(ctx => {

				var query = ctx.Query<Song>();

				return query
					.OrderBy(a => a.CreateDate.Year)
					.ThenBy(a => a.CreateDate.Month)
					.ThenBy(a => a.CreateDate.Day)
					.GroupBy(a => new {
						Year = a.CreateDate.Year, 
						Month = a.CreateDate.Month,
						Day = a.CreateDate.Day
					})
					.Select(a => new {
						a.Key.Year,
						a.Key.Month,
						a.Key.Day,
						Count = a.Count()
					})
					.Where(a => a.Count < 1000)
					.ToArray();

			});

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();

			return DateLineChartWithAverage("Songs added per day", "Songs", "Number of songs", points);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult SongsPublishedPerDay() {
			
			var values = repository.HandleQuery(ctx => {

				var query = ctx.Query<Song>();

				return query
					.Where(a => a.PublishDate.DateTime != null)
					.OrderBy(a => a.PublishDate.DateTime.Value.Year)
					.ThenBy(a => a.PublishDate.DateTime.Value.Month)
					.ThenBy(a => a.PublishDate.DateTime.Value.Day)
					.GroupBy(a => new {
						Year = a.PublishDate.DateTime.Value.Year, 
						Month = a.PublishDate.DateTime.Value.Month,
						Day = a.PublishDate.DateTime.Value.Day
					})
					.Select(a => new {
						a.Key.Year,
						a.Key.Month,
						a.Key.Day,
						Count = a.Count()
					})
					.ToArray();

			});

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();

			return DateLineChartWithAverage("Songs published per day", "Songs", "Number of songs", points);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult SongsPerGenre() {
			
			var result = GetGenreTagUsages<SongTagUsage>();
			return SimplePieChart("Songs per genre", "Songs", result);

		}

		public ActionResult SongsPerProducer() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllSongs.Count(s => !s.IsSupport && !s.Song.Deleted && s.Song.SongType == SongType.Original)
					}), "Original songs by producer", "Songs");

		}

		public ActionResult SongsPerVocaloid() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Vocaloid || a.ArtistType == ArtistType.UTAU || a.ArtistType == ArtistType.Utaite)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllSongs.Count(s => !s.IsSupport && !s.Song.Deleted)
					}), "Songs by Vocaloid/UTAU", "Songs");

		}

		public ActionResult FollowersPerProducer() {
			
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.Users.Count
					}), "Followers by producer", "Followers");

		}

		public ActionResult HitsPerAlbum() {
			
			var values = userRepository.HandleQuery(ctx => {
				
				var idsAndHits = ctx.OfType<AlbumHit>().Query()
					.GroupBy(h => h.Album.Id)
					.Select(h => new {
						Id = h.Key,
						Count = h.Count()
					})
					.OrderByDescending(h => h.Count)
					.Take(25)
					.ToArray();

				var ids = idsAndHits.Select(i => i.Id).ToArray();

				var albums = ctx.OfType<Album>().Query()
					.Where(a => ids.Contains(a.Id))
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						EntryId = a.Id
					}).ToArray();

				foreach (var hit in idsAndHits)
					albums.First(a => a.EntryId == hit.Id).Value = hit.Count;

				return albums.OrderByDescending(a => a.Value);

			});

			var categories = values.Select(p => p.Name[permissionContext.LanguagePreference]).ToArray();
			var data = values.Select(p => p.Value).ToArray();

			return SimpleBarChart("Hits per album", "Hits", categories, data);

		}

		public ActionResult HitsPerSong() {
			
			var values = userRepository.HandleQuery(ctx => {
				
				var idsAndHits = ctx.OfType<SongHit>().Query()
					.GroupBy(h => h.Song.Id)
					.Select(h => new {
						Id = h.Key,
						Count = h.Count()
					})
					.OrderByDescending(h => h.Count)
					.Take(25)
					.ToArray();

				var ids = idsAndHits.Select(i => i.Id).ToArray();

				var albums = ctx.OfType<Song>().Query()
					.Where(a => ids.Contains(a.Id))
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						EntryId = a.Id
					}).ToArray();

				foreach (var hit in idsAndHits)
					albums.First(a => a.EntryId == hit.Id).Value = hit.Count;

				return albums.OrderByDescending(a => a.Value);

			});

			var categories = values.Select(p => p.Name[permissionContext.LanguagePreference]).ToArray();
			var data = values.Select(p => p.Value).ToArray();

			return SimpleBarChart("Hits per song", "Hits", categories, data);

		}

		public ActionResult Index() {
			
			return View();

		}

	}

}