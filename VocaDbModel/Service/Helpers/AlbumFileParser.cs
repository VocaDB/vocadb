using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using VocaDb.Model.DataContracts.MikuDb;

namespace VocaDb.Model.Service.Helpers
{
	public class AlbumFileParser
	{
		private readonly Regex numRegex = new Regex(@"(\d+)");

		private string[] GetArtistNames(string artistString)
		{
			//var names = artistString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

			//if (names.Length == 1)
			var names = artistString.Split(new[] { ", ", " & " }, StringSplitOptions.RemoveEmptyEntries);

			return names;
		}

		private ImportedAlbumTrack ParseTrack(DataRow dataRow, int nextTrackNum)
		{
			var track = new ImportedAlbumTrack();

			track.Title = dataRow.GetString(AlbumFileField.Title);
			var trackNumMatch = numRegex.Match(dataRow.GetString(AlbumFileField.Track, string.Empty));
			if (trackNumMatch.Success)
				track.TrackNum = int.Parse(trackNumMatch.Groups[1].Value);
			else
				track.TrackNum = nextTrackNum;

			var artists = new List<string>();

			var composer = dataRow.GetString(AlbumFileField.Composer, string.Empty);

			if (composer != string.Empty)
				artists.Add(composer);

			var artist = dataRow.GetString(AlbumFileField.Artist, string.Empty);

			if (artist != string.Empty)
			{
				var featPos = artist.IndexOf("feat.", StringComparison.InvariantCultureIgnoreCase);

				if (featPos != -1)
				{
					var vocaloidName = artist.Substring(featPos + 5, artist.Length - featPos - 5).Trim();
					track.VocalistNames = GetArtistNames(vocaloidName);
					artist = artist.Substring(0, featPos).Trim();
				}
				else
				{
					track.VocalistNames = new string[] { };
				}

				artists.AddRange(GetArtistNames(artist));
			}

			track.ArtistNames = artists.Distinct().ToArray();
			return track;
		}

		public MikuDbAlbumContract Parse(Stream input)
		{
			var tracks = new List<ImportedAlbumTrack>();
			var parser = new DataRowParser();
			var data = new ImportedAlbumDataContract();
			data.Title = "Unknown";

			using (var reader = new StreamReader(input))
			{
				string row;
				while ((row = reader.ReadLine()) != null)
				{
					if (!parser.IsConfigured)
					{
						parser.Configure(row);
					}
					else
					{
						var dataRow = new DataRow(parser, row);

						var albumName = dataRow.GetString(AlbumFileField.Album, string.Empty);
						if (albumName != string.Empty)
							data.Title = albumName;

						var year = dataRow.GetIntOrDefault(AlbumFileField.Year, 0);
						if (year != 0)
							data.ReleaseYear = year;

						var track = ParseTrack(dataRow, tracks.Count + 1);
						tracks.Add(track);
					}
				}
			}

			data.ArtistNames = tracks.SelectMany(t => t.ArtistNames).Distinct().ToArray();
			data.VocalistNames = tracks.SelectMany(t => t.VocalistNames).Distinct().ToArray();
			data.Tracks = tracks.OrderBy(t => t.TrackNum).ToArray();

			return new MikuDbAlbumContract(data);
		}
	}

	public enum AlbumFileField
	{
		Album = 0,

		Artist = 1,

		Composer = 2,

		Title = 3,

		Track = 4,

		Year = 5
	}

	public class DataRowParser
	{
		private readonly Dictionary<AlbumFileField, int> fieldCols = new Dictionary<AlbumFileField, int>();

		public bool Configure(string headerRow)
		{
			fieldCols.Clear();
			var cols = headerRow.Split(';');

			if (!cols.Any(c => Enum.IsDefined(typeof(AlbumFileField), c)))
				return false;

			for (int i = 0; i < cols.Length; ++i)
			{
				AlbumFileField field;
				if (Enum.TryParse(cols[i], true, out field))
				{
					fieldCols.Add(field, i);
				}
			}

			return true;
		}

		public string GetFieldOrEmpty(string[] cols, AlbumFileField field)
		{
			if (!IsConfigured)
				throw new InvalidOperationException("Field column indices not configured");

			if (!fieldCols.ContainsKey(field))
				return string.Empty;

			var index = fieldCols[field];

			if (cols.Length <= index)
				return string.Empty;

			return cols[index];
		}

		public bool IsConfigured
		{
			get
			{
				return fieldCols.Any();
			}
		}
	}

	public class DataRow
	{
		private readonly string[] cols;
		private readonly DataRowParser rowParser;

		public DataRow(DataRowParser rowParser, string row)
		{
			this.rowParser = rowParser;
			this.cols = row.Split(';');
		}

		public int GetIntOrDefault(AlbumFileField field, int def)
		{
			var val = rowParser.GetFieldOrEmpty(cols, field);

			if (val == string.Empty)
				return def;

			int i;

			if (int.TryParse(val, out i))
				return i;

			return def;
		}

		public string GetString(AlbumFileField field)
		{
			var val = rowParser.GetFieldOrEmpty(cols, field);

			if (val == string.Empty)
				throw new InvalidOperationException("Required field " + field + " is empty");

			return val;
		}

		public string GetString(AlbumFileField field, string def)
		{
			var val = rowParser.GetFieldOrEmpty(cols, field);

			if (val == string.Empty)
				return def;

			return val;
		}
	}
}
