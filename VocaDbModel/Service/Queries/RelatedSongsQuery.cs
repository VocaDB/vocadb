#nullable disable

using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;

namespace VocaDb.Model.Service.Queries;

public class RelatedSongsQuery
{
	private readonly IDatabaseContext<Song> _ctx;

	private Artist[] GetMainArtists(Song song, IList<IArtistLinkWithRoles> creditableArtists)
	{
		return ArtistHelper.GetProducers(creditableArtists, SongHelper.GetContentFocus(song.SongType)).Select(a => a.Artist).ToArray();
	}

#nullable enable
	public RelatedSongsQuery(IDatabaseContext<Song> ctx)
	{
		ParamIs.NotNull(() => ctx);

		_ctx = ctx;
	}
#nullable disable

	public int[] GetLikeMatches(Song song, IList<int> ignoreIds, int count)
	{
		if (song.RatingScore <= 0)
		{
			return Array.Empty<int>();
		}

		// N users who rated this song the highest
		var userIds = song.UserFavorites
			.OrderByDescending(r => r.Rating)
			.Take(20)
			.Select(u => u.User.Id)
			.ToArray();

		return _ctx
			.Query<FavoriteSongForUser>()
			.Where(f =>
				userIds.Contains(f.User.Id)
				&& !ignoreIds.Contains(f.Song.Id)
				&& !f.Song.Deleted)
			.GroupBy(f => f.Song.Id)
			.Select(f => new
			{
				SongId = f.Key,
				Ratings = f.Sum(r => (int)r.Rating),
				SongScore = f.Sum(r => r.Song.RatingScore)
			})
			.OrderByDescending(f => f.Ratings)
			.ThenByDescending(f => f.SongScore)
			.Select(s => s.SongId)
			.Take(count)
			.ToArray();
	}

#nullable enable
	public RelatedSongs GetRelatedSongs(Song song, SongRelationsFields fields = SongRelationsFields.All, int count = 12)
	{
		ParamIs.NotNull(() => song);

		var songs = new RelatedSongs();
		var songId = song.Id;
		var loadedSongs = new List<int>(count * 2) { songId };

		if (fields.HasFlag(SongRelationsFields.ArtistMatches))
		{
			var creditableArtists = song.Artists.Where(a => a.Artist != null && !a.IsSupport).ToArray();

			var mainArtists = GetMainArtists(song, creditableArtists);

			if (mainArtists != null && mainArtists.Any())
			{
				var mainArtistIds = mainArtists.Select(a => a.Id).ToArray();
				var songsByMainArtists = _ctx.Query()
					.Where(s =>
						s.Id != songId
						&& !s.Deleted
						&& s.AllArtists.Any(a =>
							!a.IsSupport
							&& mainArtistIds.Contains(a.Artist.Id)))
					.OrderBy(SongSortRule.RatingScore)
					.Take(count)
					.ToArray();

				songs.ArtistMatches = songsByMainArtists;
				loadedSongs.AddRange(songsByMainArtists.Select(s => s.Id));
			}
		}

		if (fields.HasFlag(SongRelationsFields.LikeMatches) && song.RatingScore > 0)
		{
			var likeMatches = GetLikeMatches(song, loadedSongs, count);

			songs.LikeMatches = _ctx.Query().Where(s => likeMatches.Contains(s.Id)).ToArray();
			loadedSongs.AddRange(likeMatches);
		}

		if (fields.HasFlag(SongRelationsFields.TagMatches) && song.Tags.Tags.Any())
		{
			// Take top 5 tags
			var tagIds = song.Tags.Usages
				.OrderByDescending(u => u.Count)
				.Take(5)
				.Select(t => t.Tag.Id)
				.ToArray();

			var songsWithTags =
				_ctx.Query().Where(s =>
					s.Id != songId
					&& !loadedSongs.Contains(s.Id)
					&& !s.Deleted
					&& s.Tags.Usages.Any(t => tagIds.Contains(t.Tag.Id)))
				.OrderBy(SongSortRule.RatingScore)
				.Take(count)
				.ToArray();

			songs.TagMatches = songsWithTags;
		}

		return songs;
	}
#nullable disable
}

public class RelatedSongs
{
	public RelatedSongs()
	{
		ArtistMatches = Array.Empty<Song>();
		LikeMatches = Array.Empty<Song>();
		TagMatches = Array.Empty<Song>();
	}

	public Song[] ArtistMatches { get; set; }

	public Song[] LikeMatches { get; set; }

	public Song[] TagMatches { get; set; }
}

[Flags]
public enum SongRelationsFields
{
	None = 0,
	ArtistMatches = 1 << 0,
	LikeMatches = 1 << 1,
	TagMatches = 1 << 2,
	All = ArtistMatches | LikeMatches | TagMatches,
}
