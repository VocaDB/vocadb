using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongDetailsContract {

		public SongDetailsContract() {}

		public SongDetailsContract(Song song, ContentLanguagePreference languagePreference,
			SongListBaseContract[] pools, int changedLyricsTagId) {

			Song = new SongContract(song, languagePreference);

			AdditionalNames = string.Join(", ", song.AllNames.Where(n => n != Song.Name).Distinct());
			Albums = song.OnAlbums.Select(a => new AlbumContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			AlternateVersions = song.AlternateVersions.Select(s => new SongContract(s, languagePreference)).ToArray();
			Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			ArtistString = song.ArtistString[languagePreference];
			CreateDate = song.CreateDate;
			Deleted = song.Deleted;
			LikeCount = song.UserFavorites.Count(f => f.Rating == SongVoteRating.Like);
			LyricsFromParents = song.GetLyricsFromParents(changedLyricsTagId).Select(l => new LyricsForSongContract(l)).ToArray();
			Notes = song.Notes;
			OriginalVersion = (song.OriginalVersion != null && !song.OriginalVersion.Deleted ? 
				new SongForApiContract(song.OriginalVersion, null, languagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl) : null);

			PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
			Tags = song.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).OrderByDescending(t => t.Count).ToArray();
			TranslatedName = new TranslatedStringContract(song.TranslatedName);
			WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

			Pools = pools;

		}

		/// <summary>
		/// Album id of the album being browsed.
		/// 0 if none.
		/// </summary>
		public AlbumContract Album { get; set; }

		[DataMember]
		public AlbumContract[] Albums { get; set; }

		[DataMember]
		public SongInAlbumContract AlbumSong { get; set; }

		[DataMember]
		public SongContract[] AlternateVersions { get; set; }

		[DataMember]
		public string AdditionalNames { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public string ArtistString { get; set; }

		[DataMember]
		public int CommentCount { get; set; }

		[DataMember]
		public DateTime CreateDate { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public int Hits { get; set; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

		[DataMember]
		public int LikeCount { get; set; }

		[DataMember]
		public int ListCount { get; set; }

		[DataMember]
		public LyricsForSongContract[] LyricsFromParents { get; set; }

		[DataMember]
		public SongContract MergedTo { get; set; }

		/// <summary>
		/// Next song on the album being browsed (identified by AlbumId).
		/// Can be null.
		/// </summary>
		[DataMember]
		public SongInAlbumContract NextSong { get; set; }

		[DataMember]
		public EnglishTranslatedString Notes { get; set; }

		[DataMember]
		public SongForApiContract OriginalVersion { get; set; }

		[DataMember]
		public SongListBaseContract[] Pools { get; set; }

		/// <summary>
		/// Previous song on the album being browsed (identified by AlbumId).
		/// Can be null.
		/// </summary>
		[DataMember]
		public SongInAlbumContract PreviousSong { get; set; }

		[DataMember(Name = "pvs")]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public SongContract Song { get; set; }

		[DataMember]
		public TagUsageForApiContract[] Tags { get; set; }

		[DataMember]
		public TranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public SongVoteRating UserRating { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

	}

}
