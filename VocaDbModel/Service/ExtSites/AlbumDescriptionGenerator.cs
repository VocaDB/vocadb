#nullable disable

using System;
using System.Globalization;
using System.Linq;
using System.Text;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.ExtSites
{
	public class AlbumDescriptionGenerator
	{
		private void AddBasicDescription(StringBuilder sb, AlbumContract album, Func<DiscType, string> albumTypeNames)
		{
			var typeName = albumTypeNames(album.DiscType);

			sb.Append(typeName);

			if (!album.ReleaseDate.IsEmpty)
			{
				var date = OptionalDateTime.Create(album.ReleaseDate).ToString(CultureInfo.InvariantCulture);
				sb.AppendFormat(", released {0}", date);
			}
		}

		public string GenerateDescription(AlbumContract album, Func<DiscType, string> albumTypeNames)
		{
			var sb = new StringBuilder();
			AddBasicDescription(sb, album, albumTypeNames);
			sb.Append(".");
			return sb.ToString();
		}

		public string GenerateDescription(ServerOnlyAlbumDetailsContract album, Func<DiscType, string> albumTypeNames)
		{
			var sb = new StringBuilder();

			AddBasicDescription(sb, album, albumTypeNames);

			if (album.Songs.Any())
			{
				sb.AppendFormat(", {0} track(s)", album.Songs.Length);

				if (album.TotalLength != TimeSpan.Zero)
				{
					sb.AppendFormat(" ({0})", DateTimeHelper.FormatMinSec(album.TotalLength));
				}
			}

			sb.Append(".");

			return sb.ToString();
		}
	}
}
