#nullable disable

using VocaDb.Model.Domain.PVs;

namespace VocaDb.Model.Service.Search.AlbumSearch;

public class ParsedAlbumQuery
{
	public ParsedAlbumQuery()
	{
	}

	public int Id { get; set; }

	public int ArtistId { get; set; }

	public SearchTextQuery Name { get; set; }

	public string TagName { get; set; }

	public IPV PV { get; set; }

	public bool HasNameQuery => !SearchTextQuery.IsNullOrEmpty(Name);
}
