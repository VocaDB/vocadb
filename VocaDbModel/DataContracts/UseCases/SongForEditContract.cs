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
			MinMilliBpm = song.MinMilliBpm;
			MaxMilliBpm = song.MaxMilliBpm;
		}

		/// <summary>
		/// ID of the first album's release event.
		/// Used for validation warnings.
		/// </summary>
		[DataMember]
		public int? AlbumEventId { get; init; }

		[DataMember]
		public DateTime? AlbumReleaseDate { get; init; }

		[DataMember]
		public ArtistForSongContract[] Artists { get; set; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; init; }

		/// <summary>
		/// Song is on one or more albums
		/// </summary>
		[DataMember]
		public bool HasAlbums { get; init; }

		[DataMember]
		public LyricsForSongContract[] Lyrics { get; set; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; init; }

		[DataMember]
		public EnglishTranslatedStringContract Notes { get; init; }

		[DataMember]
		public SongContract OriginalVersion { get; init; }

		[DataMember(Name = "pvs")]
		public PVContract[] PVs { get; set; }

		[DataMember]
		public ReleaseEventContract ReleaseEvent { get; set; }

		// Required here for validation
		[DataMember]
		public int[] Tags { get; init; }

		[DataMember]
		public string UpdateNotes { get; set; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; set; }

		[DataMember]
		public int? MinMilliBpm { get; init; }

		[DataMember]
		public int? MaxMilliBpm { get; init; }
	}
}
