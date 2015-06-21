using System;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.ExtSites {

	public class AlbumDescriptionGenerator {

		private void AddBasicDescription(StringBuilder sb, AlbumContract album, Func<DiscType, string> albumTypeNames) {
			
			var typeName = albumTypeNames(album.DiscType);

			sb.Append(typeName);

			if (!album.ReleaseDate.IsEmpty)
				sb.AppendFormat(", released {0}", album.ReleaseDate.Formatted);

		}

		public string GenerateDescription(AlbumContract album, Func<DiscType, string> albumTypeNames) {
					
			var sb = new StringBuilder();
			AddBasicDescription(sb, album, albumTypeNames);
			return sb.ToString();

		}

		public string GenerateDescription(AlbumDetailsContract album, Func<DiscType, string> albumTypeNames) {
					
			var sb = new StringBuilder();

			AddBasicDescription(sb, album, albumTypeNames);

			if (album.Songs.Any()) {

				sb.AppendFormat(", {0} track(s)", album.Songs.Length);

				if (album.TotalLength != TimeSpan.Zero) {
					sb.AppendFormat(" ({0}:{1})", (int)album.TotalLength.TotalMinutes, album.TotalLength.Seconds);
				}

			}

			return sb.ToString();

		}

	}

}
