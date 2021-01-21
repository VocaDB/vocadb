#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.UseCases
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongForEditContract : SongContract
	{
		public SongForEditContract() { }

		public SongForEditContract(Song song, ContentLanguagePreference languagePreference)
			: base(song, languagePreference)
		{
			ParamIs.NotNull(() => song);

			var firstAlbum = song.Albums.Where(a => a.Album.OriginalReleaseDate.IsFullDate).OrderBy(a => a.Album.OriginalReleaseDate).FirstOrDefault();

			AlbumEventId = firstAlbum?.Album.OriginalReleaseEvent?.Id;
			AlbumReleaseDate = song.FirstAlbumDate != null ? (DateTime?)DateTime.SpecifyKind(song.FirstAlbumDate.Value, DateTimeKind.Utc) : null;
			Artists = song.Artists.Select(a => new ArtistForSongContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			DefaultNameLanguage = song.TranslatedName.DefaultLanguage;
			HasAlbums = song.Albums.Any();
			Lyrics = song.Lyrics.Select(l => new LyricsForSongContract(l)).ToArray();
			Names = song.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			Notes = new EnglishTranslatedStringContract(song.Notes);
			OriginalVersion = (song.OriginalVersion != null && !song.OriginalVersion.Deleted ? new SongContract(song.OriginalVersion, languagePreference) : null);
			PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
			ReleaseEvent = song.ReleaseEvent != null ? new ReleaseEventContract(song.ReleaseEvent, languagePreference) : null;
			Tags = song.Tags.Tags.Select(t => t.Id).ToArray();
			UpdateNotes = string.Empty;
			WebLinks = song.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
			Bpm = song.Bpm;
			MaxBpm = song.MaxBpm;
		}

		/// <summary>
		/// ID of the first album's release event.
		/// Used for validation warnings.
		/// </summary>
		[DataMember]
		public int? AlbumEventId { get; set; }

		[DataMember]
		public DateTime? AlbumReleaseDate { get; set; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		/// <summary>
		/// Song is on one or more albums
		/// </summary>
		[DataMember]
		public bool HasAlbums { get; set; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public EnglishTranslatedStringContract Notes { get; set; }

		[DataMember]
		public SongContract OriginalVersion { get; set; }

		[DataMember(Name = "pvs")]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public ReleaseEventContract ReleaseEvent { get; set; }

		// Required here for validation
		[DataMember]
		public int[] Tags { get; set; }

		[DataMember]
		public string UpdateNotes { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

		[DataMember]
		public int? Bpm { get; set; }

		[DataMember]
		public int? MaxBpm { get; set; }
	}
}
