#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Aggregate;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Caching;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code.Highcharts;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
	public class StatsController : ControllerBase
	{
		private const int ClientCacheDurationSec = 86400;
		private readonly VdbConfigManager _config;
		private readonly ActivityEntryQueries _activityEntryQueries;
		private readonly StatsQueries _queries;
		private readonly SongAggregateQueries _songAggregateQueries;
		private readonly ObjectCache _cache;

		private ActionResult AreaChart(string title, params Series[] dataSeries)
		{
			var json = new Highchart
			{
				Chart = new Chart
				{
					Height = 600,
					Type = ChartType.Area
				},
				Title = title,
				XAxis = new Axis(AxisType.Datetime, new Title()),
				YAxis = new Axis
				{
					Title = "Percentage",
					Min = 0
				},
				Tooltip = new
				{
					Shared = true,
					Crosshairs = true
				},
				PlotOptions = new PlotOptions
				{
					Bar = new
					{
						DataLabels = new
						{
							Enabled = true
						}
					},
					Area = new PlotOptionsArea
					{
						Stacking = PlotOptionsAreaStacking.Percent,
						LineColor = "#ffffff",
						LineWidth = 1,
						Marker = new
						{
							LineWidth = 1,
							LineColor = "#ffffff"
						}
					}
				},
				Legend = new
				{
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
			bool average = true)
		{
			Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
			{
				Public = true,
				MaxAge = TimeSpan.FromDays(1),
			};
			// TODO: implement Response.Cache.SetSlidingExpiration(true);

			return LowercaseJson(HighchartsHelper.DateLineChartWithAverage(title, pointsTitle, yAxisTitle, points, average));
		}

		private ActionResult SimpleBarChart(string title, string seriesName, IList<string> categories, IList<int> data)
		{
			Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
			{
				Public = true,
				MaxAge = TimeSpan.FromDays(1),
			};
			// TODO: implement Response.Cache.SetSlidingExpiration(true);

			return Json(new
			{
				chart = new
				{
					type = "bar",
					height = 600
				},
				title = new
				{
					text = title
				},
				xAxis = new
				{
					categories,
					title = new
					{
						text = (string)null
					}
				},
				yAxis = new
				{
					title = new
					{
						text = seriesName
					}
				},
				plotOptions = new
				{
					bar = new
					{
						dataLabels = new
						{
							enabled = true
						}
					}
				},
				legend = new
				{
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

		private ActionResult SimpleBarChart<T>(Func<IQueryable<T>, IQueryable<StatsQueries.LocalizedValue>> func, string title, string seriesName)
			where T : class, IDatabaseObject
		{
			var values = GetTopValues(func);

			var categories = values.Select(p => p.Name[_permissionContext.LanguagePreference]).ToArray();
			var data = values.Select(p => p.Value).ToArray();

			return SimpleBarChart(title, seriesName, categories, data);
		}

		private ActionResult SimplePieChart(string title, string seriesName, ICollection<Tuple<string, int>> points)
		{
			Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
			{
				Public = true,
				MaxAge = TimeSpan.FromDays(1),
			};
			// TODO: implement Response.Cache.SetSlidingExpiration(true);

			return LowercaseJson(HighchartsHelper.SimplePieChart(title, seriesName, points, false));
		}


		private ICollection<Tuple<string, int>> GetGenreTagUsages<T>() where T : TagUsage
		{
			return _userRepository.HandleQuery(ctx =>
			{
				var genres = ctx.OfType<T>()
					.Query()
					.Where(u => u.Tag.Parent == null && u.Tag.CategoryName == TagCommonCategoryNames.Genres)
					.GroupBy(s => s.Tag.Id)
					.Select(g => new
					{
						TagId = g.Key,
						Count = g.Count()
					})
					.OrderByDescending(g => g.Count)
					.ToArray();

				var mainGenreIds = genres.OrderByDescending(t => t.Count).Take(10).Select(t => t.TagId).ToArray();
				var mainGenreTags = ctx.Query<Tag>().Where(t => mainGenreIds.Contains(t.Id)).SelectIdAndName(PermissionContext.LanguagePreference).ToDictionary(t => t.Id);
				var sorted = genres.Select(t => new
				{
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

		private StatsQueries.LocalizedValue[] GetTopValues<T>(Func<IQueryable<T>, IQueryable<StatsQueries.LocalizedValue>> func)
			where T : class, IDatabaseObject
		{
			var name = $"{ControllerContext.RouteData.Values["action"]}_{ControllerContext.RouteData.Values["cutoff"]}";
			return _cache.GetOrInsert($"report_{name}", new CacheItemPolicy { SlidingExpiration = TimeSpan.FromDays(1), Priority = CacheItemPriority.Default }, () =>
			{
				var data = _userRepository.HandleQuery(ctx =>
				{
					return func(ctx.OfType<T>().Query())
						.OrderByDescending(a => a.Value)
						.Take(25)
						.ToArray();
				});
				return data;
			});
		}

		private readonly IUserPermissionContext _permissionContext;
		private readonly IUserRepository _userRepository;

		private DateTime DefaultMinDate => new DateTime(_config.SiteSettings.MinAlbumYear, 1, 1);

		public StatsController(
			IUserRepository userRepository,
			IUserPermissionContext permissionContext,
			SongAggregateQueries songAggregateQueries,
			VdbConfigManager config,
			ActivityEntryQueries activityEntryQueries,
			StatsQueries queries,
			ObjectCache cache
		)
		{
			_userRepository = userRepository;
			_permissionContext = permissionContext;
			_activityEntryQueries = activityEntryQueries;
			_queries = queries;
			_songAggregateQueries = songAggregateQueries;
			_config = config;
			_cache = cache;
		}

		[ResponseCache(Duration = ClientCacheDurationSec)]
		public ActionResult AlbumsPerGenre()
		{
			var points = GetGenreTagUsages<AlbumTagUsage>();
			return SimplePieChart("Albums per genre", "Albums", points);
		}

		[ResponseCache(Duration = ClientCacheDurationSec)]
		public ActionResult AlbumsPerMonth()
		{
			var now = DateTime.Now;

			var values = _userRepository.HandleQuery(ctx =>
			{
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
							|| r.Artist.ArtistType == ArtistType.Utaite
							|| r.Artist.ArtistType == ArtistType.SynthesizerV))
					.OrderBy(a => a.OriginalRelease.ReleaseDate.Year)
					.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
					.GroupBy(a => new
					{
						Year = a.OriginalRelease.ReleaseDate.Year,
						Month = a.OriginalRelease.ReleaseDate.Month
					})
					.Select(a => new
					{
						a.Key.Year,
						a.Key.Month,
						Count = a.Count()
					})
					.ToArray();
			});

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year.Value, v.Month.Value, 1), v.Count)).ToArray();

			return DateLineChartWithAverage("Releases by month", "Albums", "Albums released", points);
		}

		public ActionResult AlbumsPerProducer()
		{
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new StatsQueries.LocalizedValue
					{
						Name = new TranslatedString
						{
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English,
							Romaji = a.Names.SortNames.Romaji,
							Japanese = a.Names.SortNames.Japanese,
						},
						Value = a.AllAlbums.Count(s => !s.IsSupport && !s.Album.Deleted && s.Album.DiscType != DiscType.Compilation),
						EntryId = a.Id
					}), "Albums by producer", "Songs");
		}

		public ActionResult AlbumsPerVocaloid(DateTime? cutoff)
		{
			Expression<Func<ArtistForAlbum, bool>> dateFilter = (song) => (cutoff.HasValue ? song.Album.CreateDate >= cutoff : true);

			return SimpleBarChart<Artist>(q => q
					.Where(a =>
						a.ArtistType == ArtistType.Vocaloid ||
						a.ArtistType == ArtistType.UTAU ||
						a.ArtistType == ArtistType.CeVIO ||
						a.ArtistType == ArtistType.Utaite ||
						a.ArtistType == ArtistType.SynthesizerV)
					.Select(a => new StatsQueries.LocalizedValue
					{
						Name = new TranslatedString
						{
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

		[ResponseCache(Duration = ClientCacheDurationSec)]
		public ActionResult AlbumSongsOverTime()
		{
			var data = _songAggregateQueries.SongsOverTime(TimeUnit.Month, false, null, a => a.AllAlbums.Any(), a => a.AllAlbums.Count == 0);

			return AreaChart("Album songs over time",
				new Series("Album songs", Series.DateData(data[0])),
				new Series("Independent songs", Series.DateData(data[1]))
			);
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult ArtistsPerMonth(DateTime? cutoff = null)
		{
			cutoff ??= DefaultMinDate;

			var values = _queries.ArtistsPerMonth(cutoff);

			var points = values.ToDatePoints();

			return DateLineChartWithAverage("Active artists per month", "Artists", "Number of artists", points, true);
		}

		[ResponseCache(Duration = ClientCacheDurationSec)]
		public ActionResult CumulativeAlbums()
		{
			var values = _userRepository.HandleQuery(ctx =>
			{
				return ctx.Query<Album>()
					.WhereNotDeleted()
					.WhereHasReleaseDate()
					.OrderByReleaseDate(SortDirection.Ascending)
					.GroupBy(a => new
					{
						Year = a.OriginalRelease.ReleaseDate.Year,
						Month = a.OriginalRelease.ReleaseDate.Month,
						Day = a.OriginalRelease.ReleaseDate.Day
					})
					.Select(a => new CountPerDayContract
					{
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

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult CumulativeSongsPublished(DateTime? cutoff)
		{
			var values = _songAggregateQueries.SongsOverTime(TimeUnit.Month, false, cutoff, a => a.PVs.PVs.Any(), a => a.PVs.PVs.Count == 0).First();

			var points = values.CumulativeSum();

			return DateLineChartWithAverage("Cumulative songs published per day", "Songs", "Number of songs", points, false);
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult EditsPerDay(DateTime? cutoff)
		{
			var points = _activityEntryQueries.GetEditsPerDay(null, cutoff);

			return DateLineChartWithAverage("Edits per day", "Edits", "Number of edits", points);
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult EditsPerUser(DateTime? cutoff)
		{
			return SimpleBarChart<ActivityEntry>(q =>
			{
				return q
					.FilterIfNotNull(cutoff, a => a.CreateDate >= cutoff.Value)
					.GroupBy(a => a.Author.Name)
					.Select(a => new StatsQueries.LocalizedValue
					{
						Name = new TranslatedString
						{
							DefaultLanguage = ContentLanguageSelection.Japanese,
							Japanese = a.Key
						},
						Value = a.Count(),
					});
			}, "Edits per user", "User");
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult PVsPerService(DateTime? cutoff, bool onlyOriginal = false)
		{
			var result = _userRepository.HandleQuery(ctx =>
			{
				var pvs = ctx.Query<PVForSong>()
					.FilterIfNotNull(cutoff, pv => pv.PublishDate >= cutoff)
					.Where(pv => !onlyOriginal || pv.PVType == PVType.Original)
					.GroupBy(s => s.Service)
					.Select(g => new
					{
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

		[ResponseCache(Duration = ClientCacheDurationSec)]
		public ActionResult PVsPerServiceOverTime()
		{
			var data = _userRepository.HandleQuery(ctx =>
			{
				return ctx.Query<PVForSong>()
					.Where(a => a.PublishDate != null)
					.Where(pv => pv.PVType == PVType.Original)
					.OrderBy(a => a.PublishDate.Value.Year)
					.ThenBy(a => a.PublishDate.Value.Month)
					.GroupBy(a => new
					{
						Service = a.Service,
						Year = a.PublishDate.Value.Year,
						Month = a.PublishDate.Value.Month
					})
					.Select(a => new
					{
						a.Key.Service,
						a.Key.Year,
						a.Key.Month,
						Count = a.Count()
					})
					.ToArray();
			});

			var dataWithDateTime = data.Select(d => new { d.Service, Date = new DateTime(d.Year, d.Month, 1), d.Count }).ToArray();

			var byService = dataWithDateTime.GroupBy(d => d.Service);

			var dataSeries = byService.Select(ser => new Series
			{
				//type = "line",
				Name = ser.Key.ToString(),
				Data = Series.DateData(ser, p => p.Date, p => p.Count)
			}).ToArray();

			return AreaChart("Original PVs per service over time", dataSeries);
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult SongsAddedPerDay(DateTime? cutoff)
		{
			var values = _queries.SongsAddedPerDay(cutoff);

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();

			return DateLineChartWithAverage("Songs added per day", "Songs", "Number of songs", points);
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult SongsPublishedPerDay(DateTime? cutoff = null, TimeUnit unit = TimeUnit.Day)
		{
			cutoff ??= DefaultMinDate;
			var values = _songAggregateQueries.SongsOverTime(unit, false, cutoff, s => s.PublishDate.DateTime <= DateTime.Now, null)[0];

			var points = values.Select(v => Tuple.Create(new DateTime(v.Year, v.Month, v.Day), v.Count)).ToArray();

			return DateLineChartWithAverage("Songs published per " + unit.ToString().ToLowerInvariant(), "Songs", "Number of songs", points);
		}

		[ResponseCache(Duration = ClientCacheDurationSec)]
		public ActionResult SongsPerGenre()
		{
			var result = GetGenreTagUsages<SongTagUsage>();
			return SimplePieChart("Songs per genre", "Songs", result);
		}

		public ActionResult SongsPerProducer()
		{
			var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;

			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new StatsQueries.LocalizedValue
					{
						Name = new TranslatedString
						{
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

		public ActionResult SongsPerVocaloid(DateTime? cutoff)
		{
			Expression<Func<ArtistForSong, bool>> dateFilter = (song) => (cutoff.HasValue ? song.Song.CreateDate >= cutoff : true);

			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Vocaloid || a.ArtistType == ArtistType.UTAU || a.ArtistType == ArtistType.Utaite || a.ArtistType == ArtistType.SynthesizerV)
					.Select(a => new StatsQueries.LocalizedValue
					{
						Name = new TranslatedString
						{
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English,
							Romaji = a.Names.SortNames.Romaji,
							Japanese = a.Names.SortNames.Japanese,
						},
						Value = a.AllSongs.AsQueryable().Where(dateFilter)
							.Count(s => !s.IsSupport && !s.Song.Deleted)
					}), "Songs by Vocaloid/UTAU", "Songs");
		}

		public ActionResult SongsPerVocaloidOverTime(DateTime? cutoff, ArtistType[] vocalistTypes = null, int startYear = 2007)
		{
			if (vocalistTypes == null)
				vocalistTypes = new[] { ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.OtherVoiceSynthesizer, ArtistType.SynthesizerV };

			var data = _queries.SongsPerVocaloidOverTime(cutoff, vocalistTypes, startYear);

			var dataSeries = data.Select(ser => new Series
			{
				Name = ser.Item1.Names.SortNames.English,
				Data = Series.DateData(ser.Item2, p => p.Date, p => p.Count)
			}).ToArray();

			return AreaChart("Songs per voicebank over time", dataSeries);
		}

		[ResponseCache(Duration = ClientCacheDurationSec, VaryByQueryKeys = new[] { "*" })]
		public ActionResult GetSongsPerVoicebankTypeOverTime(DateTime? cutoff, ArtistType[] vocalistTypes = null, int startYear = 2007)
		{
			if (vocalistTypes == null)
				vocalistTypes = new[] { ArtistType.Vocaloid, ArtistType.UTAU, ArtistType.CeVIO, ArtistType.OtherVoiceSynthesizer, ArtistType.SynthesizerV };

			var data = _queries.GetSongsPerVoicebankTypeOverTime(cutoff, vocalistTypes, startYear);

			var dataSeries = data
				.Select(ser => new Series
				{
					Name = Translate.ArtistTypeName(ser.Key),
					Data = Series.DateData(ser, p => p.Item1, p => p.Item3)
				})
				.ToArray();

			return AreaChart("Songs per vocalist type over time", dataSeries);
		}

		[ResponseCache(Duration = ClientCacheDurationSec)]
		public ActionResult SongsWithoutPVOverTime()
		{
			var data = _songAggregateQueries.SongsOverTime(TimeUnit.Month, false, null, a => a.PVs.PVs.Any(), a => a.PVs.PVs.Count == 0);

			return AreaChart("Songs with and without PV over time",
				new Series("Songs with a PV", Series.DateData(data[0])),
				new Series("Songs without a PV", Series.DateData(data[1]))
			);
		}

		public ActionResult FollowersPerProducer()
		{
			return SimpleBarChart<Artist>(q => q
					.Where(a => a.ArtistType == ArtistType.Producer)
					.Select(a => new StatsQueries.LocalizedValue
					{
						Name = new TranslatedString
						{
							DefaultLanguage = a.Names.SortNames.DefaultLanguage,
							English = a.Names.SortNames.English,
							Romaji = a.Names.SortNames.Romaji,
							Japanese = a.Names.SortNames.Japanese,
						},
						Value = a.Users.Count
					}), "Followers by producer", "Followers");
		}

		public ActionResult HitsPerAlbum(DateTime? cutoff)
		{
			var values = _userRepository.HandleQuery(ctx =>
			{
				var idsAndHits = ctx.OfType<AlbumHit>().Query()
					.FilterIfNotNull(cutoff, s => s.Date > cutoff)
					.GroupBy(h => h.Entry.Id)
					.Select(h => new
					{
						Id = h.Key,
						Count = h.Count()
					})
					.OrderByDescending(h => h.Count)
					.Take(25)
					.ToArray();

				var ids = idsAndHits.Select(i => i.Id).ToArray();

				var albums = ctx.OfType<Album>().Query()
					.Where(a => ids.Contains(a.Id))
					.Select(a => new StatsQueries.LocalizedValue
					{
						Name = new TranslatedString
						{
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

			var categories = values.Select(p => p.Name[_permissionContext.LanguagePreference]).ToArray();
			var data = values.Select(p => p.Value).ToArray();

			return SimpleBarChart("Hits per album", "Hits", categories, data);
		}

		public ActionResult HitsPerSong(DateTime? cutoff)
		{
			var values = _userRepository.HandleQuery(ctx =>
			{
				var idsAndHits = ctx.OfType<SongHit>().Query()
					.FilterIfNotNull(cutoff, s => s.Date > cutoff)
					.GroupBy(h => h.Entry.Id)
					.Select(h => new
					{
						Id = h.Key,
						Count = h.Count()
					})
					.OrderByDescending(h => h.Count)
					.Take(25)
					.ToArray();

				var ids = idsAndHits.Select(i => i.Id).ToArray();

				var songs = StatsQueries.GetSongsWithNames(ctx, ids).Values;

				foreach (var hit in idsAndHits)
					songs.First(a => a.EntryId == hit.Id).Value = hit.Count;

				return songs.OrderByDescending(a => a.Value);
			});

			var categories = values.Select(p => p.Name[_permissionContext.LanguagePreference]).ToArray();
			var data = values.Select(p => p.Value).ToArray();

			return SimpleBarChart("Views per song", "Hits", categories, data);
		}

		public ActionResult HitsPerSongOverTime(DateTime? cutoff)
		{
			var data = _queries.HitsPerSongOverTime(cutoff);

			var dataSeries = data.Select(ser => new Series
			{
				Name = ser.Entry.Name.English,
				Data = Series.DateData(ser.Data.CumulativeSumContract())
			}).ToArray();

			return LowercaseJson(HighchartsHelper.DateLineChart("Views per song over time", "Songs", "Views", dataSeries));
		}

		public ActionResult ScorePerSongOverTime(DateTime? cutoff)
		{
			var data = _queries.ScorePerSongOverTime(cutoff);

			var dataSeries = data.Select(ser => new Series
			{
				Name = ser.Entry.Name.English,
				Data = Series.DateData(ser.Data.CumulativeSumContract())
			}).ToArray();

			return LowercaseJson(HighchartsHelper.DateLineChart("Score per song over time", "Songs", "Score", dataSeries));
		}

		private static Dictionary<int, StatsQueries.LocalizedValue> GetSongsWithNamesAndArtists(IDatabaseContext ctx, int[] topSongIds)
		{
			var songs = ctx.OfType<Song>().Query()
				.Where(a => topSongIds.Contains(a.Id))
				.Select(a => new StatsQueries.LocalizedValue
				{
					Name = new TranslatedString
					{
						DefaultLanguage = a.Names.SortNames.DefaultLanguage,
						English = a.Names.SortNames.English + " (" + a.ArtistString.English + ")",
						Romaji = a.Names.SortNames.Romaji + " (" + a.ArtistString.Romaji + ")",
						Japanese = a.Names.SortNames.Japanese + " (" + a.ArtistString.Japanese + ")",
					},
					EntryId = a.Id
				}).ToDictionary(s => s.EntryId);
			return songs;
		}

		public ActionResult UsersPerLanguage()
		{
			return SimpleBarChart<UserKnownLanguage>(q => q
					.Where(u => u.CultureCode.CultureCode != null && u.CultureCode.CultureCode != string.Empty)
					.GroupBy(u => u.CultureCode)
					.ToArray()
					.Select(u => new StatsQueries.LocalizedValue
					{
						Name = TranslatedString.Create(u.Key.CultureInfo.Name),
						Value = u.Count(),
					}).AsQueryable(),
				"Users per language", "Users");
		}

		public ActionResult Index()
		{
			return View("React/Index");
		}
	}
}