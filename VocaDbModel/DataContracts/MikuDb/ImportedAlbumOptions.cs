#nullable disable

using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.DataContracts.MikuDb
{
	public class ImportedAlbumOptions
	{
		public ImportedAlbumOptions(int importedAlbumId)
		{
			ImportedDbAlbumId = importedAlbumId;
		}

		public ImportedAlbumOptions(InspectedAlbum inspectedAlbum)
		{
			ImportedDbAlbumId = inspectedAlbum.ImportedAlbum.Id;
			MergedAlbumId = inspectedAlbum.MergedAlbumId;
			MergeTracks = inspectedAlbum.MergeTracks;
			SelectedLanguage = inspectedAlbum.SelectedLanguage;
		}

		public int ImportedDbAlbumId { get; init; }

		public int? MergedAlbumId { get; init; }

		public bool? MergeTracks { get; init; }

		public ContentLanguageSelection SelectedLanguage { get; init; }
	}
}
