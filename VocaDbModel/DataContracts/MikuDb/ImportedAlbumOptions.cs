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

		public int ImportedDbAlbumId { get; set; }

		public int? MergedAlbumId { get; set; }

		public bool? MergeTracks { get; set; }

		public ContentLanguageSelection SelectedLanguage { get; set; }
	}
}
