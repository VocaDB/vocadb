#nullable disable

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	public class ParsedAlbumQuery
	{
		public ParsedAlbumQuery()
		{
		}

		public int ArtistId { get; set; }

		public string Name { get; set; }

		public string TagName { get; set; }

		public bool HasNameQuery => !string.IsNullOrEmpty(Name);
	}
}
