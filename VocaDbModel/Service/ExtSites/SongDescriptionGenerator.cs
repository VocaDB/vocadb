using System;
using System.Globalization;
using System.Text;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Service.ExtSites
{
	public class SongDescriptionGenerator
	{
		private void AddBasicDescription(StringBuilder sb, SongContract song, TranslateableEnum<SongType> songTypeNames)
		{
			var typeName = songTypeNames.GetName(song.SongType, CultureInfo.InvariantCulture);

			sb.Append(typeName);

			if (!string.IsNullOrEmpty(song.ArtistString))
				sb.AppendFormat(" by {0}", song.ArtistString);

			if (song.PublishDate.HasValue)
				sb.AppendFormat(", published {0}", song.PublishDate.Value.ToShortDateString());
		}

		public string GenerateDescription(SongContract song, TranslateableEnum<SongType> songTypeNames)
		{
			var sb = new StringBuilder();

			AddBasicDescription(sb, song, songTypeNames);

			return sb.ToString();
		}
	}
}
