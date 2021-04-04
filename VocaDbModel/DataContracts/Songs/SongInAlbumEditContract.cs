#nullable disable

using System;
using System.Linq;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class SongInAlbumEditContract
	{
		public SongInAlbumEditContract() { }

#nullable enable
		public SongInAlbumEditContract(SongInAlbum songInAlbum, ContentLanguagePreference languagePreference)
		{
			ParamIs.NotNull(() => songInAlbum);

			DiscNumber = songInAlbum.DiscNumber;
			SongInAlbumId = songInAlbum.Id;
			TrackNumber = songInAlbum.TrackNumber;

			ArtistString = string.Empty;
			SongAdditionalNames = string.Empty;
			SongId = 0;

			var song = songInAlbum.Song;
			if (song != null)
			{
				Artists = song.ArtistList.Select(a => new ArtistContract(a, languagePreference)).ToArray();
				ArtistString = song.ArtistString[languagePreference];
				SongName = song.TranslatedName[languagePreference];
				SongAdditionalNames = string.Join(", ", song.AllNames.Where(n => n != SongName));
				SongId = song.Id;
			}
			else
			{
				Artists = new ArtistContract[0];
				SongName = songInAlbum.Name;
			}

			IsCustomTrack = song == null;
		}
#nullable disable

		public ArtistContract[] Artists { get; set; }

		public string ArtistString { get; init; }

		public int DiscNumber { get; init; }

		public bool IsCustomTrack { get; init; }

		public string SongAdditionalNames { get; init; }

		public int SongId { get; init; }

		public int SongInAlbumId { get; init; }

		public string SongName { get; init; }

		public int TrackNumber { get; set; }
	}
}
