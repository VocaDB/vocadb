#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Search;

namespace VocaDb.Model.Service.QueryableExtensions
{
	public static class SongQueryableExtensions
	{
		public static IOrderedQueryable<Song> OrderByPublishDate(this IQueryable<Song> criteria, SortDirection direction)
		{
			return criteria.OrderBy(a => a.PublishDate, direction)
				.ThenBy(a => a.CreateDate, direction);
		}

		public static IQueryable<Song> OrderBy(this IQueryable<Song> query, SongSortRule sortRule,
			ContentLanguagePreference languagePreference = ContentLanguagePreference.Default, int tagId = 0) => sortRule switch
		{
			SongSortRule.Name => query.OrderByEntryName(languagePreference),
			SongSortRule.AdditionDate => query.OrderByDescending(a => a.CreateDate),
			SongSortRule.FavoritedTimes => query.OrderByDescending(a => a.FavoritedTimes),
			SongSortRule.PublishDate => query.OrderByPublishDate(SortDirection.Descending),
			SongSortRule.RatingScore => query.OrderByDescending(a => a.RatingScore),
			SongSortRule.TagUsageCount => query.OrderByTagUsage(tagId),
			_ => query,
		};

		public static IQueryable<Song> OrderBy(
			this IQueryable<Song> query, EntrySortRule sortRule, ContentLanguagePreference languagePreference) => sortRule switch
		{
			EntrySortRule.Name => query.OrderByEntryName(languagePreference),
			EntrySortRule.AdditionDate => query.OrderByDescending(a => a.CreateDate),
			EntrySortRule.ActivityDate => query.OrderByDescending(a => a.PublishDate.DateTime),
			_ => query,
		};

		public static IMaybeOrderedQueryable<Song> OrderByTagUsage(this IQueryable<Song> query, int tagId)
		{
			return query.OrderByTagUsage<Song, SongTagUsage>(tagId);
		}

		public static IQueryable<Song> WhereArtistHasTag(this IQueryable<Song> query, string tagName)
		{
			if (string.IsNullOrEmpty(tagName))
				return query;

			return query.Where(s => s.AllArtists.Any(a => a.Artist.Tags.Usages.Any(t => t.Tag.Names.SortNames.English == tagName || t.Tag.Names.SortNames.Romaji == tagName || t.Tag.Names.SortNames.Japanese == tagName)));
		}

		public static IQueryable<Song> WhereArtistHasType(this IQueryable<Song> query, ArtistType artistType)
		{
			return query.WhereArtistHasType<Song, ArtistForSong>(artistType);
		}

		public static IQueryable<Song> WhereArtistIsFollowedByUser(this IQueryable<Song> query, int userId)
		{
			if (userId == 0)
				return query;

			query = query.Where(s => s.AllArtists.Any(a => a.Artist.Users.Any(u => u.User.Id == userId)));

			return query;
		}

		public static IQueryable<Song> WhereDraftsOnly(this IQueryable<Song> query, bool draftsOnly)
		{
			if (!draftsOnly)
				return query;

			return query.Where(a => a.Status == EntryStatus.Draft);
		}

		/// <summary>
		/// Filters a song query by a list of artist Ids.
		/// </summary>
		public static IQueryable<Song> WhereHasArtist(this IQueryable<Song> query, int artistId)
		{
			if (artistId == 0)
				return query;

			return query.WhereHasArtist<Song, ArtistForSong>(artistId, false, false);
		}

		public static IQueryable<Song> WhereHasArtistParticipationStatus(this IQueryable<Song> query,
			ArtistParticipationQueryParams queryParams,
			IEntityLoader<Artist> artistGetter)
		{
			var various = Model.Helpers.ArtistHelper.VariousArtists;
			var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;
			var artistId = queryParams.ArtistIds.Primary;

			return EntryWithArtistsQueryableExtensions.WhereHasArtistParticipationStatus(new ArtistParticipationQueryParams<Song, ArtistForSong>(query, queryParams, artistGetter,
				al => al.AllArtists.Any(a => a.Artist.Id == artistId && !a.IsSupport && ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default)) && a.Song.ArtistString.Default != various),
				al => al.AllArtists.Any(a => a.Artist.Id == artistId && (a.IsSupport || ((a.Roles != ArtistRoles.Default) && ((a.Roles & producerRoles) == ArtistRoles.Default)) || a.Song.ArtistString.Default == various))
			));
		}

		public static IQueryable<Song> WhereHasLyrics(this IQueryable<Song> query, string[] languageCodes, bool any)
		{
			if (any)
			{
				return query.Where(s => s.Lyrics.Any());
			}
			else if (languageCodes != null && languageCodes.Any())
			{
				return query.Where(s => s.Lyrics.Any(l => languageCodes.Contains(l.CultureCode.CultureCode)));
			}
			else
			{
				return query;
			}
		}

		public static IQueryable<Song> WhereHasLyricsContent(this IQueryable<Song> query, string text)
		{
			if (string.IsNullOrEmpty(text))
				return query;

			return query.Where(s => s.Lyrics.Any(l => l.Value.Contains(text)));
		}

		/// <summary>
		/// Filters a song query by a name query.
		/// </summary>
		/// <param name="query">Song query. Cannot be null.</param>
		/// <param name="nameFilter">Name filter string. If null or empty, no filtering is done.</param>
		/// <param name="matchMode">Desired mode for matching names.</param>
		/// <param name="words">
		/// List of words for the words search mode. 
		/// Can be null, in which case the words list will be parsed from <paramref name="nameFilter"/>.
		/// </param>
		/// <returns>Filtered query. Cannot be null.</returns>
		public static IQueryable<Song> WhereHasName(this IQueryable<Song> query, SearchTextQuery textQuery)
		{
			return query.WhereHasNameGeneric<Song, SongName>(textQuery);
		}

		public static IQueryable<Song> WhereHasName(this IQueryable<Song> query, IEnumerable<SearchTextQuery> names)
		{
			return query.WhereHasNameGeneric<Song, SongName>(names);
		}

		public static IQueryable<Song> WhereCreateDateIsWithin(this IQueryable<Song> criteria, TimeSpan timeFilter)
		{
			if (timeFilter == TimeSpan.Zero)
				return criteria;

			var since = DateTime.Now - timeFilter;

			return criteria.Where(t => t.CreateDate >= since);
		}

		public static IQueryable<Song> WhereHasNicoId(this IQueryable<Song> query, string nicoId)
		{
			if (string.IsNullOrEmpty(nicoId))
				return query;

			return query.Where(s => s.NicoId == nicoId);
		}

		public static IQueryable<Song> WhereHasParentSong(this IQueryable<Song> query, int parentSongId)
		{
			if (parentSongId == 0)
				return query;

			query = query.Where(s => s.OriginalVersion.Id == parentSongId
				|| s.OriginalVersion.OriginalVersion.Id == parentSongId
				|| s.OriginalVersion.OriginalVersion.OriginalVersion.Id == parentSongId);

			return query;
		}

		public static IQueryable<Song> WhereHasPublishDate(this IQueryable<Song> query, bool hasPublishDate)
		{
			return hasPublishDate ? query.Where(s => s.PublishDate.DateTime != null) : query.Where(s => s.PublishDate.DateTime == null);
		}

		/// <summary>
		/// Filter query by PV services bit array.
		/// Song will pass the filter if ANY of the specified PV services matches.
		/// </summary>
		/// <param name="query">Query. Cannot be null.</param>
		/// <param name="pvServices">PV services bit array. Can be null, in which case no filtering will be done.</param>
		/// <returns>Filtered query.</returns>
		public static IQueryable<Song> WhereHasPVService(this IQueryable<Song> query, PVServices? pvServices)
		{
			if (pvServices == null || pvServices.Value == PVServices.Nothing)
				return query;

			return query.Where(s => (s.PVServices & pvServices) != PVServices.Nothing);
		}

		/// <summary>
		/// Filter query including only songs with a PV.
		/// </summary>
		public static IQueryable<Song> WhereHasPV(this IQueryable<Song> query)
		{
			return query.Where(t => t.PVServices != PVServices.Nothing);
		}

		public static IQueryable<Song> WhereHasPV(this IQueryable<Song> criteria, bool onlyWithPVs)
		{
			return onlyWithPVs ? WhereHasPV(criteria) : criteria;
		}

		public static IQueryable<Song> WhereHasPV(this IQueryable<Song> query, PVService? service, string pvId)
		{
			if (service == null || pvId == null)
				return query;

			return query.Where(s => s.PVs.PVs.Any(pv => pv.Service == service && pv.PVId == pvId));
		}

		public static IQueryable<Song> WhereHasPV(this IQueryable<Song> query, IPV pv)
		{
			if (pv == null)
				return query;

			return WhereHasPV(query, pv.Service, pv.PVId);
		}

		public static IQueryable<Song> WhereHasScore(this IQueryable<Song> query, int minScore)
		{
			if (minScore <= 0)
				return query;

			return query.Where(q => q.RatingScore >= minScore);
		}

		public static IQueryable<Song> WhereHasTag(this IQueryable<Song> query, string tagName)
		{
			return query.WhereHasTag<Song, SongTagUsage>(tagName);
		}

		public static IQueryable<Song> WhereHasTags(this IQueryable<Song> query, string[] tagName)
		{
			return query.WhereHasTags<Song, SongTagUsage>(tagName);
		}

		public static IQueryable<Song> WhereHasTags(this IQueryable<Song> query, int[] tagId, bool childTags = false)
		{
			return query.WhereHasTags<Song, SongTagUsage>(tagId, childTags);
		}

		public static IQueryable<Song> WhereHasType(this IQueryable<Song> query, SongType[] songTypes)
		{
			if (!songTypes.Any())
				return query;

			return query.Where(m => songTypes.Contains(m.SongType));
		}

		public static IQueryable<Song> WhereHasTypeOrTag(this IQueryable<Song> query, EntryTypeAndTagCollection<SongType> entryTypeAndTagCollection)
		{
			if (entryTypeAndTagCollection == null || entryTypeAndTagCollection.IsEmpty)
				return query;

			if (!entryTypeAndTagCollection.SubTypes.Any())
				return query.Where(song => song.Tags.Usages.Any(u => entryTypeAndTagCollection.TagIds.Contains(u.Tag.Id)));

			return query.Where(song => entryTypeAndTagCollection.SubTypes.Contains(song.SongType)
				|| song.Tags.Usages.Any(u => entryTypeAndTagCollection.TagIds.Contains(u.Tag.Id)));
		}

		public static IQueryable<Song> WhereHasType(this IQueryable<Song> query, SongType? songType)
		{
			if (songType == null)
				return query;

			return query.Where(s => s.SongType == songType);
		}

		public static IQueryable<Song> WhereHasVocalist(this IQueryable<Song> query, SongVocalistSelection vocalist) => vocalist switch
		{
			SongVocalistSelection.Vocaloid => query.Where(s => s.AllArtists.Any(a => !a.IsSupport && a.Artist.ArtistType == ArtistType.Vocaloid)),
			SongVocalistSelection.UTAU => query.Where(s => s.AllArtists.Any(a => !a.IsSupport && a.Artist.ArtistType == ArtistType.UTAU)),
			SongVocalistSelection.Other => query.Where(s => s.AllArtists.Any(a => !a.IsSupport && (a.Artist.ArtistType == ArtistType.CeVIO || a.Artist.ArtistType == ArtistType.OtherVoiceSynthesizer || a.Artist.ArtistType == ArtistType.SynthesizerV))),
			_ => query,
		};

		public static IQueryable<Song> WhereIdIs(this IQueryable<Song> query, int id)
		{
			if (id == 0)
				return query;

			return query.Where(m => m.Id == id);
		}

		public static IQueryable<Song> WhereIdNotIn(this IQueryable<Song> query, int[] ignoreIds)
		{
			if (ignoreIds == null || !ignoreIds.Any())
				return query;

			return query.Where(s => !ignoreIds.Contains(s.Id));
		}

		public static IQueryable<Song> WhereInUserCollection(this IQueryable<Song> query, int userId)
		{
			if (userId == 0)
				return query;

			query = query.Where(s => s.UserFavorites.Any(a => a.User.Id == userId));

			return query;
		}

		public static IQueryable<Song> WhereMatchFilter(this IQueryable<Song> query, AdvancedSearchFilter filter)
		{
			if (filter == null)
				return query;

			switch (filter.FilterType)
			{
				case AdvancedFilterType.ArtistType:
					{
						var param = EnumVal<ArtistType>.Parse(filter.Param);
						return WhereArtistHasType(query, param);
					}
				case AdvancedFilterType.HasAlbum:
					return filter.Negate ? query.Where(s => !s.AllAlbums.Any()) : query.Where(s => s.AllAlbums.Any());
				case AdvancedFilterType.HasMultipleVoicebanks:
					{
						return query.Where(s => s.AllArtists.Count(a => !a.IsSupport && ArtistHelper.VoiceSynthesizerTypes.Contains(a.Artist.ArtistType)) > 1);
					}
				case AdvancedFilterType.HasPublishDate:
					{
						return query.WhereHasPublishDate(!filter.Negate);
					}
				case AdvancedFilterType.Lyrics:
					{
						var any = filter.Param == AdvancedSearchFilter.Any;
						var languageCodes = !any ? (filter.Param ?? string.Empty).Split(',') : null;
						return WhereHasLyrics(query, languageCodes, any);
					}
				case AdvancedFilterType.LyricsContent:
					{
						return query.WhereHasLyricsContent(filter.Param);
					}
				case AdvancedFilterType.HasOriginalMedia:
					{
						return query.Where(s => filter.Negate != s.PVs.PVs.Any(pv => !pv.Disabled && pv.PVType == PVType.Original));
					}
				case AdvancedFilterType.HasMedia:
					{
						return query.Where(s => filter.Negate != s.PVs.PVs.Any());
					}
				case AdvancedFilterType.WebLink:
					{
						return query.WhereHasLink<Song, SongWebLink>(filter.Param);
					}
			}

			return query;
		}

		public static IQueryable<Song> WhereMatchFilters(this IQueryable<Song> query, IEnumerable<AdvancedSearchFilter> filters)
		{
			return filters?.Aggregate(query, WhereMatchFilter) ?? query;
		}

		public static IQueryable<Song> WherePublishDateIsBetween(this IQueryable<Song> query, DateTime? begin, DateTime? end)
		{
			if (begin.HasValue && end.HasValue)
				return query.Where(e => e.PublishDate.DateTime != null && e.PublishDate.DateTime >= begin && e.PublishDate.DateTime < end);

			if (begin.HasValue)
				return query.Where(e => e.PublishDate.DateTime != null && e.PublishDate.DateTime >= begin);

			if (end.HasValue)
				return query.Where(e => e.PublishDate.DateTime != null && e.PublishDate.DateTime < end);

			return query;
		}

		public static IQueryable<Song> WhereReleaseEventIs(this IQueryable<Song> query, int eventId)
		{
			if (eventId == 0)
				return query;

			return query.Where(s => s.ReleaseEvent.Id == eventId);
		}

		public static IQueryable<Song> WhereMilliBpmIsBetween(this IQueryable<Song> query, int? minMilliBpm, int? maxMilliBpm)
		{
			if (minMilliBpm.HasValue && maxMilliBpm.HasValue)
				return query.Where(s => s.MinMilliBpm != null && s.MaxMilliBpm != null && s.MinMilliBpm >= minMilliBpm && s.MaxMilliBpm <= maxMilliBpm);

			if (minMilliBpm.HasValue)
				return query.Where(s => s.MinMilliBpm != null && s.MinMilliBpm >= minMilliBpm);

			if (maxMilliBpm.HasValue)
				return query.Where(s => s.MaxMilliBpm != null && s.MaxMilliBpm <= maxMilliBpm);

			return query;
		}
	}

	public enum SongVocalistSelection
	{
		Nothing,
		Vocaloid,
		UTAU,
		Other,
	}
}
