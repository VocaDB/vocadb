using System.Globalization;
using System.Text;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.ExtSites;

public class AlbumDescriptionGenerator
{
	private void AddBasicDescription(StringBuilder sb, AlbumDetailsForApiContract album, Func<DiscType, string?> albumTypeNames)
	{
		var typeName = albumTypeNames(album.DiscType);

		sb.Append(typeName);

		if (album.OriginalRelease is not null && album.OriginalRelease.ReleaseDate is not null && !album.OriginalRelease.ReleaseDate.IsEmpty)
		{
			var date = OptionalDateTime.Create(album.OriginalRelease.ReleaseDate).ToString(CultureInfo.InvariantCulture);
			sb.AppendFormat(", released {0}", date);
		}
	}

	public string GenerateDescription(AlbumDetailsForApiContract album, Func<DiscType, string?> albumTypeNames)
	{
		var sb = new StringBuilder();

		AddBasicDescription(sb, album, albumTypeNames);

		if (album.Songs.Any())
		{
			sb.AppendFormat(", {0} track(s)", album.Songs.Length);

			if (album.TotalLengthSeconds != 0)
			{
				sb.AppendFormat(" ({0})", DateTimeHelper.FormatMinSec(album.TotalLengthSeconds));
			}
		}

		sb.Append(".");

		return sb.ToString();
	}
}
