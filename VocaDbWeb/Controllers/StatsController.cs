using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Aggregate;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Web.Code.Highcharts;
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

		public static Tuple<DateTime, int>[] CumulativeSum(this IEnumerable<CountPerDayContract> sequence) {
			return sequence.CumulativeSelect<CountPerDayContract, Tuple<DateTime, int>>((v, previous) => {
				return Tuple.Create(v.ToDateTime(), (previous != null ? previous.Item2 : 0) + v.Count);
			}).ToArray();
		}

		public static Tuple<DateTime, int>[] ToDatePoints(this IEnumerable<CountPerDayContract> sequence) {
			return sequence.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();
		}
	}

	class SongsPerArtistPerDate {

		public SongsPerArtistPerDate() {}

		public SongsPerArtistPerDate(DateTime date, int artistId, int count) {
			Date = date;
			ArtistId = artistId;
			Count = count;
		}

		public DateTime Date { get; set; }

		public int ArtistId { get; set; }

		public int Count { get; set; }

	}

	public class StatsController : ControllerBase {

		private const int clientCacheDurationSec = 86400;
		private readonly SongAggregateQueries songAggregateQueries;

		private T GetCachedReport<T>() where T : class {

			var name = ControllerContext.RouteData.Values["action"] + "_" + ControllerContext.RouteData.Values["cutoff"];
			var item = context.Cache["report_" + name];

			if (item == null)
				return null;

			return (T)item;

		}

		private void SaveCachedReport<T>(T data) where T : class {
			
			var name = ControllerContext.RouteData.Values["action"];
			context.Cache.Add("report_" + name, data, null, Cache.NoAbsoluteExpiration, TimeSpan.FromDays(1), CacheItemPriority.Default, null);

		}

		private int GetRootVb(int vb, Dictionary<int, int> voicebankMap) {

			int loops = 0;
			while (loops++ <= 10) {
				if (!voicebankMap.ContainsKey(vb))
					return vb;
				vb = voicebankMap[vb];
			}

			return vb;

		}

		private SongsPerArtistPerDate[] SumToBaseVoicebanks(IDatabaseContext ctx, SongsPerArtistPerDate[] data) {

			var baseVoicebankMap = ctx.Query<Artist>().Where(a => a.BaseVoicebank != null).ToDictionary(a => a.Id, a => a.BaseVoicebank.Id);

			var changed = true;
			while (changed) {
				changed = false;
				foreach (var vb in baseVoicebankMap) {
					var rootVb = GetRootVb(vb.Value, baseVoicebankMap);
					if (vb.Value != rootVb) {
						baseVoicebankMap[vb.Key] = rootVb;
						changed = true;
						break;
					}
				}
			}

			var dataDict = data
				.GroupBy(d => d.Date)
				.ToDictionary(byDate => byDate.Key, byDate => byDate
					.GroupBy(byDate2 => baseVoicebankMap.ContainsKey(byDate2.ArtistId) ? baseVoicebankMap[byDate2.ArtistId] : byDate2.ArtistId)
					.ToDictionary(byArtist => byArtist.Key, byArtist => byArtist
						.Select(d3 => d3.Count)
						.Sum()));

			return dataDict.SelectMany(d => d.Value.Select(d2 => new SongsPerArtistPerDate(d.Key, d2.Key, d2.Value))).ToArray();

		}

		class LocalizedValue {

			public int EntryId { get; set; }

			public TranslatedString Name { get; set; }

			public int Value { get; set; }

		}

		private ActionResult AreaChart(string title, params Series[] dataSeries) {

			var json = new Highchart {
				Chart = new Chart {
					Height = 600,
					Type = ChartType.Area
				},
				Title = title,
				XAxis = new Axis(AxisType.Datetime, new Title()),
				YAxis = new Axis {
					Title = "Percentage",
					Min = 0
				},
				Tooltip = new {
					Shared = true,
					Crosshairs = true
				},
				PlotOptions = new PlotOptions {
					Bar = new {
						DataLabels = new {
							Enabled = true
						}
					},
					Area = new PlotOptionsArea {
						Stacking = PlotOptionsAreaStacking.Percent,
						LineColor = "#ffffff",
						LineWidth = 1,
						Marker = new {
							LineWidth = 1,
							LineColor = "#ffffff"
						}
					}
				},
				Legend = new {
					Layout = "vertical",
					Align = "left",
					X = 120,
					VerticalAlign = "top",
					Y = 100,
					Floating = true,
					BackgroundColor = "#FFFFFF"
				},
				Series = (
					dataSeries
				)
			};

			return LowercaseJson(json);

		}

		private ActionResult DateLineChartWithAverage(string title, string pointsTitle, string yAxisTitle, ICollection<Tuple<DateTime, int>> points,
			bool average = true) {
			
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetMaxAge(TimeSpan.FromDays(1));
			Response.Cache.SetSlidingExpiration(true);

			return LowercaseJson(HighchartsHelper.DateLineChartWithAverage(title, pointsTitle, yAxisTitle, points, average));

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

			return LowercaseJson(HighchartsHelper.SimplePieChart(title, seriesName, points, false));

		}


		private ICollection<Tuple<string, int>> GetGenreTagUsages<T>() where T : TagUsage {
			
			return userRepository.HandleQuery(ctx => {
				
				var genres = ctx.OfType<T>()
					.Query()
					.Where(u => u.Tag.AliasedTo == null && u.Tag.Parent == null && u.Tag.CategoryName == TagCommonCategoryNames.Genres)
					.GroupBy(s => s.Tag.Id)
					.Select(g => new {
						TagId = g.Key,
						Count = g.Count()
					})
					.OrderByDescending(g => g.Count)
					.ToArray();

				var mainGenreIds = genres.OrderByDescending(t => t.Count).Take(10).Select(t => t.TagId).ToArray();
				var mainGenreTags = ctx.Query<Tag>().Where(t => mainGenreIds.Contains(t.Id)).SelectIdAndName(PermissionContext.LanguagePreference).ToDictionary(t => t.Id);
				var sorted = genres.Select(t => new {
					TagName = mainGenreTags.ContainsKey(t.TagId) ? mainGenreTags[t.TagId].Name : null,
					Count = t.Count
				}).OrderByDescending(t => t.Count);

				var mainGenres = sorted.Take(10).ToArray();
				var otherCount = sorted.Skip(10).Sum(g => g.Count);
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

		public StatsController(IUserRepository userRepository, IRepository repository, IUserPermissionContext permissionContext, SongAggregateQueries songAggregateQueries,
			HttpContextBase context) {

			this.userRepository = userRepository;
			this.repository = repository;
			this.permissionContext = permissionContext;
			this.songAggregateQueries = songAggregateQueries;
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

		public ActionResult AlbumsPerVocaloid(DateTime? cutoff) {

			Expression<Func<ArtistForAlbum, bool>> dateFilter = (song) => (cutoff.HasValue ? song.Album.CreateDate >= cutoff : true);

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
						Value = a.AllAlbums
							.AsQueryable().Where(dateFilter)
							.Count(s => !s.IsSupport && !s.Album.Deleted)
					}), "Albums by Vocaloid/UTAU", "Songs");

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult AlbumSongsOverTime() {

			var data = songAggregateQueries.SongsOverTime(TimeUnit.Month, false, null, a => a.AllAlbums.Any(), a => a.AllAlbums.Count == 0);

			return AreaChart("Album songs over time",
				new Series("Album songs", Series.DateData(data[0])),
				new Series("Independent songs", Series.DateData(data[1]))
			);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult ArtistsPerMonth() {

			// TODO: report not verified
			var values = repository.HandleQuery(ctx => {

				return ctx.Query<ArtistForSong>()
					.Where(a => a.Artist.ArtistType == ArtistType.Producer && !a.Song.Deleted && a.Song.PublishDate.DateTime != null)
					.OrderBy(a => a.Song.PublishDate.DateTime.Value.Year)
					.ThenBy(a => a.Song.PublishDate.DateTime.Value.Month)
					.GroupBy(a => new { // Note: we want to do count distinct here, but it's not supported by NHibernate LINQ, so doing a second group by in memory
						Year = a.Song.PublishDate.DateTime.Value.Year,
						Month = a.Song.PublishDate.DateTime.Value.Month,
						Artist = a.Artist.Id
					})
					.Select(a => new {
						Year = a.Key.Year,
						Month = a.Key.Month,
						Artist = a.Key.Artist
					})
					.ToArray()
					.GroupBy(a => new {
						Year = a.Year,
						Month = a.Month
					})
					.Select(a => new CountPerDayContract {
						Year = a.Key.Year,
						Month = a.Key.Month,
						Count = a.Count()
					});

			});

			var points = values.ToDatePoints();

			return DateLineChartWithAverage("Active artists per month", "Artists", "Number of artists", points, true);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult CumulativeAlbums() {
			
			var values = userRepository.HandleQuery(ctx => {

				return ctx.Query<Album>()
					.WhereHasReleaseDate()
					.OrderByReleaseDate(SortDirection.Ascending)
					.GroupBy(a => new {
						Year = a.OriginalRelease.ReleaseDate.Year, 
						Month = a.OriginalRelease.ReleaseDate.Month,
						Day = a.OriginalRelease.ReleaseDate.Day
					})
					.Select(a => new CountPerDayContract {
						Year = a.Key.Year.Value,
						Month = a.Key.Month.Value,
						Day = a.Key.Day.Value,
						Count = a.Count()
					})
					.ToArray();

			});

			var points = values.CumulativeSum();

			return DateLineChartWithAverage("Cumulative albums per day", "Albums", "Number of albums", points, false);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult CumulativeSongsPublished(DateTime? cutoff) {

			var values = songAggregateQueries.SongsOverTime(TimeUnit.Month, false, cutoff, a => a.PVs.PVs.Any(), a => a.PVs.PVs.Count == 0).First();

			var points = values.CumulativeSum();

			return DateLineChartWithAverage("Cumulative songs published per day", "Songs", "Number of songs", points, false);

		}

		[OutputCache(Duration = clientCacheDurationSec, VaryByParam = "cutoff")]
		public ActionResult EditsPerDay(DateTime? cutoff) {
			
			var points = new ActivityEntryQueries(repository).GetEditsPerDay(null, cutoff);

			return DateLineChartWithAverage("Edits per day", "Edits", "Number of edits", points);

		}

		[OutputCache(Duration = clientCacheDurationSec, VaryByParam = "cutoff")]
		public ActionResult EditsPerUser(DateTime? cutoff) {
			
			return SimpleBarChart<ActivityEntry>(q => { 
				
				return q
					.FilterIfNotNull(cutoff, a => a.CreateDate >= cutoff.Value)
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

		[OutputCache(Duration = clientCacheDurationSec, VaryByParam = "cutoff,onlyOriginal")]
		public ActionResult PVsPerService(DateTime? cutoff, bool onlyOriginal = false) {

			var result = userRepository.HandleQuery(ctx => {

				var pvs = ctx.Query<PVForSong>()
					.FilterIfNotNull(cutoff, pv => pv.PublishDate >= cutoff)
					.Where(pv => !onlyOriginal || pv.PVType == PVType.Original)
					.GroupBy(s => s.Service)
					.Select(g => new {
						Service = g.Key,
						Count = g.Count()
					})
					.OrderByDescending(g => g.Count)
					.ToArray()
					.Select(g => Tuple.Create(g.Service.ToString(), g.Count)).ToArray();

				return pvs;

			});

			return SimplePieChart("PVs per service", "PVs", result);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult PVsPerServiceOverTime() {

			var data = userRepository.HandleQuery(ctx => {

				return ctx.Query<PVForSong>()
					.Where(a => a.PublishDate != null)
					.Where(pv => pv.PVType == PVType.Original)
					.OrderBy(a => a.PublishDate.Value.Year)
					.ThenBy(a => a.PublishDate.Value.Month)
					.GroupBy(a => new {
						Service = a.Service,
						Year = a.PublishDate.Value.Year,
						Month = a.PublishDate.Value.Month
					})
					.Select(a => new {
						a.Key.Service,
						a.Key.Year,
						a.Key.Month,
						Count = a.Count()
					})
					.ToArray();

			});

			var dataWithDateTime = data.Select(d => new { d.Service, Date = new DateTime(d.Year, d.Month, 1), d.Count }).ToArray();

			var byService = dataWithDateTime.GroupBy(d => d.Service);

			var dataSeries = byService.Select(ser => new Series {
				//type = "line",
				Name = ser.Key.ToString(),
				Data = Series.DateData(ser, p => p.Date, p => p.Count)
			}).ToArray();

			return AreaChart("Original PVs per service over time", dataSeries);

		}

		[OutputCache(Duration = clientCacheDurationSec, VaryByParam = "cutoff")]
		public ActionResult SongsAddedPerDay(DateTime? cutoff) {
			
			var values = repository.HandleQuery(ctx => {

				var query = ctx.Query<Song>();

				if (cutoff.HasValue)
					query = query.Where(a => a.CreateDate >= cutoff);

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

		[OutputCache(Duration = clientCacheDurationSec, VaryByParam = "unit")]
		public ActionResult SongsPublishedPerDay(DateTime? cutoff = null, TimeUnit unit = TimeUnit.Day) {
			
			var values = songAggregateQueries.SongsOverTime(unit, false, cutoff, s => s.PublishDate.DateTime <= DateTime.Now, null)[0];

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();

			return DateLineChartWithAverage("Songs published per " + unit.ToString().ToLowerInvariant(), "Songs", "Number of songs", points);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult SongsPerGenre() {
			
			var result = GetGenreTagUsages<SongTagUsage>();
			return SimplePieChart("Songs per genre", "Songs", result);

		}

		public ActionResult SongsPerProducer() {

			var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;

			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllSongs.Count(s => 
							!s.IsSupport && 
							!s.Song.Deleted && 
							s.Song.SongType == SongType.Original &&
							(s.Roles == ArtistRoles.Default || (s.Roles & producerRoles) != ArtistRoles.Default))
					}), "Original composed/arranged songs by producer", "Songs");

		}

		public ActionResult SongsPerVocaloid(DateTime? cutoff) {

			Expression<Func<ArtistForSong, bool>> dateFilter = (song) => (cutoff.HasValue ? song.Song.CreateDate >= cutoff : true);

			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Vocaloid || a.ArtistType == ArtistType.UTAU || a.ArtistType == ArtistType.Utaite)
					.Select(a => new LocalizedValue {
						Name = new TranslatedString {			
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English, 
							Romaji = a.Names.SortNames.Romaji, 
							Japanese = a.Names.SortNames.Japanese, 
						},
						Value = a.AllSongs.AsQueryable().Where(dateFilter)
							.Count(s => !s.IsSupport && !s.Song.Deleted)
					}), "Songs by Vocaloid/UTAU", "Songs");

		}

		public ActionResult SongsPerVocaloidOverTime() {

			var data = repository.HandleQuery(ctx => {

				// Note: the same song may be included multiple times for different artists
				var vocalistTypes = new[] { ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.OtherVoiceSynthesizer };

				var points = ctx.Query<ArtistForSong>()
					.Where(s => !s.Song.Deleted && s.Song.PublishDate.DateTime != null && vocalistTypes.Contains(s.Artist.ArtistType))
					.OrderBy(a => a.Song.PublishDate.DateTime.Value.Year)
					.ThenBy(a => a.Song.PublishDate.DateTime.Value.Month)
					.GroupBy(s => new {
						s.Song.PublishDate.DateTime.Value.Year,
						s.Song.PublishDate.DateTime.Value.Month,
						ArtistId = s.Artist.Id,
					})
					.Select(s => new {
						s.Key.Year,
						s.Key.Month,
						s.Key.ArtistId,
						Count = s.Count()
					})
					.ToArray()
					.Select(s => new SongsPerArtistPerDate(new DateTime(s.Year, s.Month, 1), s.ArtistId, s.Count))
					.ToArray();

				points = SumToBaseVoicebanks(ctx, points);

				var artists = ctx.Query<Artist>().Where(a => vocalistTypes.Contains(a.ArtistType)).ToDictionary(a => a.Id);

				// Group by artist, select artists with top 20 most songs (as counted for the root VB)
				// Note: we're filtering artists only after summing to root VBs, because otherwise appends would be ignored
				var byArtist = points.GroupBy(p => p.ArtistId)
					.OrderByDescending(byArtist2 => byArtist2.Select(p2 => p2.Count).Sum())
					.Take(15)
					.Select(a => Tuple.Create(artists[a.Key], a.ToArray()));
				return byArtist;

			});

			var dataSeries = data.Select(ser => new Series {
				Name = ser.Item1.Names.SortNames.English,
				Data = Series.DateData(ser.Item2, p => p.Date, p => p.Count)
			}).ToArray();

			return AreaChart("Songs per voicebank over time", dataSeries);

		}

		[OutputCache(Duration = clientCacheDurationSec)]
		public ActionResult SongsWithoutPVOverTime() {

			var data = songAggregateQueries.SongsOverTime(TimeUnit.Month, false, null, a => a.PVs.PVs.Any(), a => a.PVs.PVs.Count == 0);

			return AreaChart("Songs with and without PV over time",
				new Series("Songs with a PV", Series.DateData(data[0])),
				new Series("Songs without a PV", Series.DateData(data[1]))
			);

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

		public ActionResult HitsPerAlbum(DateTime? cutoff) {
			
			var values = userRepository.HandleQuery(ctx => {
				
				var idsAndHits = ctx.OfType<AlbumHit>().Query()
					.FilterIfNotNull(cutoff, s => s.Date > cutoff)
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

		public ActionResult HitsPerSong(DateTime? cutoff) {
			
			var values = userRepository.HandleQuery(ctx => {
				
				var idsAndHits = ctx.OfType<SongHit>().Query()
					.FilterIfNotNull(cutoff, s => s.Date > cutoff)
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

	public static class IQueryableExtensionsForStats {

		public static IQueryable<T> FilterIfNotNull<T>(this IQueryable<T> query, DateTime? since, Expression<Func<T, bool>> predicate) {

			if (!since.HasValue)
				return query;

			return query.Where(predicate);

		}

	}

}