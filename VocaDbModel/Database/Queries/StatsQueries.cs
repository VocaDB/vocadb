using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Aggregate;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Database.Queries
{
	public class EntryWithIdAndData<T> : IEntryWithIntId
	{
		public T Entry { get; set; }
		public int Id { get; set; }
		public IEnumerable<CountPerDayContract> Data { get; set; }
	}

	public class SongsPerArtistPerDate
	{
		public SongsPerArtistPerDate() { }

		public SongsPerArtistPerDate(DateTime date, int artistId, int count)
		{
			Date = date;
			ArtistId = artistId;
			Count = count;
		}

		public DateTime Date { get; set; }

		public int ArtistId { get; set; }

		public int Count { get; set; }
	}

	public class StatsQueries
	{
		private readonly IRepository repository;

		public StatsQueries(IRepository repository)
		{
			this.repository = repository;
		}

		private int GetRootVb(int vb, Dictionary<int, int> voicebankMap)
		{
			int loops = 0;
			while (loops++ <= 10)
			{
				if (!voicebankMap.ContainsKey(vb))
					return vb;
				vb = voicebankMap[vb];
			}

			return vb;
		}

		private Dictionary<int, int> GetRootVoicebanksMap(IDatabaseContext ctx)
		{
			// Map from child voicebank to base voicebank
			var baseVoicebankMap = ctx.Query<Artist>()
				.Where(a => a.BaseVoicebank != null)
				.ToDictionary(a => a.Id, a => a.BaseVoicebank.Id);

			// Map from child voicebank to root voicebank (A -> B, B -> C to A -> C, B -> C)
			var rootVoicebankMap = baseVoicebankMap
				.ToDictionary(a => a.Key, a => GetRootVb(a.Value, baseVoicebankMap));

			return rootVoicebankMap;
		}

		private SongsPerArtistPerDate[] SumToBaseVoicebanks(IDatabaseContext ctx, SongsPerArtistPerDate[] data)
		{
			var baseVoicebankMap = GetRootVoicebanksMap(ctx);

			// Group by date, then by root artist
			var dataDict = data
				.GroupBy(d => d.Date)
				.ToDictionary(byDate => byDate.Key, byDate => byDate
					.GroupBy(byDate2 => baseVoicebankMap.ContainsKey(byDate2.ArtistId) ? baseVoicebankMap[byDate2.ArtistId] : byDate2.ArtistId)
					.ToDictionary(byArtist => byArtist.Key, byArtist => byArtist
						.Select(d3 => d3.Count)
						.Sum()));

			// Select new dictionary with songs grouped by root artists and dates
			return dataDict.SelectMany(d => d.Value.Select(d2 => new SongsPerArtistPerDate(d.Key, d2.Key, d2.Value))).ToArray();
		}

		public class LocalizedValue
		{
			public int EntryId { get; set; }

			public TranslatedString Name { get; set; }

			public int Value { get; set; }
		}

		public IEnumerable<CountPerDayContract> ArtistsPerMonth(DateTime? cutoff = null)
		{
			var end = DateTime.Now.AddMonths(-1);

			// TODO: report not verified
			return repository.HandleQuery(ctx =>
			{
				return ctx.Query<ArtistForSong>()
					.WhereSongHasPublishDate(true)
					.WhereSongPublishDateIsBetween(cutoff, end)
					.Where(a => a.Artist.ArtistType == ArtistType.Producer && !a.Song.Deleted)
					.OrderBy(a => a.Song.PublishDate.DateTime.Value.Year)
					.ThenBy(a => a.Song.PublishDate.DateTime.Value.Month)
					.GroupBy(a => new
					{ // Note: we want to do count distinct here, but it's not supported by NHibernate LINQ, so doing a second group by in memory
						Year = a.Song.PublishDate.DateTime.Value.Year,
						Month = a.Song.PublishDate.DateTime.Value.Month,
						Artist = a.Artist.Id
					})
					.Select(a => new
					{
						Year = a.Key.Year,
						Month = a.Key.Month,
						Artist = a.Key.Artist
					})
					.ToArray()
					.GroupBy(a => new
					{
						Year = a.Year,
						Month = a.Month
					})
					.Select(a => new CountPerDayContract
					{
						Year = a.Key.Year,
						Month = a.Key.Month,
						Count = a.Count()
					});
			});
		}

		public IEnumerable<CountPerDayContract> SongsAddedPerDay(DateTime? cutoff)
		{
			return repository.HandleQuery(ctx =>
			{
				var query = ctx.Query<Song>();

				if (cutoff.HasValue)
					query = query.Where(a => a.CreateDate >= cutoff);

				return query
					.OrderBy(a => a.CreateDate.Year)
					.ThenBy(a => a.CreateDate.Month)
					.ThenBy(a => a.CreateDate.Day)
					.GroupBy(a => new
					{
						Year = a.CreateDate.Year,
						Month = a.CreateDate.Month,
						Day = a.CreateDate.Day
					})
					.ToArray()
					.Select(a => new CountPerDayContract(a.Key.Year, a.Key.Month, a.Key.Day, a.Count()))
					.Where(a => a.Count < 1000)
					.ToArray();
			});
		}

		public IEnumerable<Tuple<Artist, SongsPerArtistPerDate[]>> SongsPerVocaloidOverTime(DateTime? cutoff, ArtistType[] vocalistTypes = null, int startYear = 2007)
		{
			return repository.HandleQuery(ctx =>
			{
				// Note: the same song may be included multiple times for different artists
				var points = ctx.Query<ArtistForSong>()
					.Where(s => !s.Song.Deleted && s.Song.PublishDate.DateTime != null && s.Song.PublishDate.DateTime.Value.Year >= startYear && vocalistTypes.Contains(s.Artist.ArtistType))
					.FilterIfNotNull(cutoff, s => s.Song.PublishDate.DateTime > cutoff)
					.OrderBy(a => a.Song.PublishDate.DateTime.Value.Year)
					.GroupBy(s => new
					{
						s.Song.PublishDate.DateTime.Value.Year,
						ArtistId = s.Artist.Id,
					})
					.Select(s => new
					{
						s.Key.Year,
						s.Key.ArtistId,
						Count = s.Count()
					})
					.ToArray()
					.Select(s => new SongsPerArtistPerDate(new DateTime(s.Year, 1, 1), s.ArtistId, s.Count))
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
		}

		public IEnumerable<IGrouping<ArtistType, Tuple<DateTime, ArtistType, int>>> GetSongsPerVoicebankTypeOverTime(DateTime? cutoff, ArtistType[] vocalistTypes = null, int startYear = 2007)
		{
			return repository.HandleQuery(ctx =>
			{
				// Note: the same song may be included multiple times for different artists
				var points = ctx.Query<ArtistForSong>()
					.Where(s => !s.Song.Deleted && s.Song.PublishDate.DateTime != null && s.Song.PublishDate.DateTime.Value.Year >= startYear && vocalistTypes.Contains(s.Artist.ArtistType))
					.FilterIfNotNull(cutoff, s => s.Song.PublishDate.DateTime > cutoff)
					.OrderBy(a => a.Song.PublishDate.DateTime.Value.Year)
					.GroupBy(s => new
					{
						s.Song.PublishDate.DateTime.Value.Year,
						s.Song.PublishDate.DateTime.Value.Month,
						ArtistType = s.Artist.ArtistType
					})
					.Select(s => new
					{
						s.Key.Year,
						s.Key.Month,
						s.Key.ArtistType,
						Count = s.Count()
					})
					.ToArray()
					.Select(s => Tuple.Create(new DateTime(s.Year, s.Month, 1), s.ArtistType, s.Count))
					.GroupBy(s => s.Item2)
					.ToArray();

				return points;
			});
		}

		public IEnumerable<EntryWithIdAndData<LocalizedValue>> HitsPerSongOverTime(DateTime? cutoff)
		{
			return repository.HandleQuery(ctx =>
			{
				var topSongIds = ctx.Query<SongHit>()
					.Where(s => !s.Entry.Deleted && s.Entry.PublishDate.DateTime != null)
					.FilterIfNotNull(cutoff, s => s.Entry.PublishDate.DateTime > cutoff)
					.GroupBy(s => new
					{
						SongId = s.Entry.Id,
					})
					.Select(s => new
					{
						s.Key.SongId,
						TotalCount = s.Count()
					})
					.OrderByDescending(s => s.TotalCount)
					.Take(20)
					.ToArray()
					.Select(s => s.SongId)
					.ToArray();

				// Note: the same song may be included multiple times for different artists
				var points = ctx.Query<SongHit>()
					.Where(s => topSongIds.Contains(s.Entry.Id))
					.OrderBy(a => a.Date.Year)
					.ThenBy(a => a.Date.Month)
					.ThenBy(a => a.Date.Day)
					.GroupBy(s => new
					{
						Year = s.Date.Year,
						Month = s.Date.Month,
						Day = s.Date.Day,
						SongId = s.Entry.Id,
					})
					.Select(s => new
					{
						s.Key.Year,
						s.Key.Month,
						s.Key.Day,
						s.Key.SongId,
						Count = s.Count()
					})
					.ToArray()
					.Select(s => new
					{
						s.SongId,
						Data = new CountPerDayContract(s.Year, s.Month, s.Day, s.Count),
					})
					.ToArray();

				var songs = GetSongsWithNames(ctx, topSongIds);

				var bySong = points.GroupBy(p => p.SongId).Select(p => new EntryWithIdAndData<LocalizedValue>
				{
					Id = p.Key,
					Entry = songs[p.Key],
					Data = p.Select(d => d.Data).ToArray()
				}).OrderByIds(topSongIds);
				return bySong;
			});
		}

		public EntryWithIdAndData<LocalizedValue>[] ScorePerSongOverTime(DateTime? cutoff)
		{
			return repository.HandleQuery(ctx =>
			{
				var topSongIds = ctx.Query<FavoriteSongForUser>()
					.Where(s => !s.Song.Deleted && s.Song.PublishDate.DateTime != null)
					.FilterIfNotNull(cutoff, s => s.Song.PublishDate.DateTime > cutoff)
					.GroupBy(s => new
					{
						SongId = s.Song.Id,
					})
					.Select(s => new
					{
						s.Key.SongId,
						TotalCount = s.Sum(s2 => (int)s2.Rating)
					})
					.OrderByDescending(s => s.TotalCount)
					.Take(20)
					.ToArray()
					.Select(s => s.SongId)
					.ToArray();

				// Note: the same song may be included multiple times for different artists
				var points = ctx.Query<FavoriteSongForUser>()
					.Where(s => topSongIds.Contains(s.Song.Id))
					.OrderBy(a => a.Date.Year)
					.ThenBy(a => a.Date.Month)
					.ThenBy(a => a.Date.Day)
					.GroupBy(s => new
					{
						Year = s.Date.Year,
						Month = s.Date.Month,
						Day = s.Date.Day,
						SongId = s.Song.Id,
					})
					.Select(s => new
					{
						s.Key.Year,
						s.Key.Month,
						s.Key.Day,
						s.Key.SongId,
						Count = s.Sum(s2 => (int)s2.Rating)
					})
					.ToArray()
					.Select(s => new
					{
						s.SongId,
						Data = new CountPerDayContract(s.Year, s.Month, s.Day, s.Count),
					})
					.ToArray();

				var songs = GetSongsWithNames(ctx, topSongIds);

				var bySong = points.GroupBy(p => p.SongId).Select(p => new EntryWithIdAndData<LocalizedValue>
				{
					Id = p.Key,
					Entry = songs[p.Key],
					Data = p.Select(d => d.Data).ToArray().AddPreviousValues(true, TimeUnit.Day, DateTime.Now).ToArray()
				}).OrderByIds(topSongIds);
				return bySong;
			});
		}

		public static Dictionary<int, LocalizedValue> GetSongsWithNames(IDatabaseContext ctx, int[] topSongIds)
		{
			var songs = ctx.OfType<Song>().Query()
				.Where(a => topSongIds.Contains(a.Id))
				.Select(a => new LocalizedValue
				{
					Name = new TranslatedString
					{
						DefaultLanguage = a.Names.SortNames.DefaultLanguage,
						English = a.Names.SortNames.English,
						Romaji = a.Names.SortNames.Romaji,
						Japanese = a.Names.SortNames.Japanese,
					},
					EntryId = a.Id
				}).ToDictionary(s => s.EntryId);
			return songs;
		}
	}

	public static class IQueryableExtensionsForStats
	{
		public static IQueryable<T> FilterIfNotNull<T>(this IQueryable<T> query, DateTime? since, Expression<Func<T, bool>> predicate)
		{
			if (!since.HasValue)
				return query;

			return query.Where(predicate);
		}
	}
}
