using Resources;
using VocaDb.Model.Domain;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Resources.Domain;

namespace VocaDb.Web.Models {

	public class GlobalSearchBoxModel {

		public GlobalSearchBoxModel()
			: this(EntryType.Artist, string.Empty) { }

		public GlobalSearchBoxModel(EntryType? objectType, string searchTerm) {

			AllObjectTypes = new TranslateableEnum<EntryType>(() => EntryTypeNames.ResourceManager, new[] {
				EntryType.Undefined, EntryType.Artist, EntryType.Album, EntryType.Song, EntryType.Tag, EntryType.User,
				EntryType.ReleaseEvent, EntryType.SongList
			});
			ObjectType = objectType ?? EntryType.Artist;
			GlobalSearchTerm = searchTerm ?? string.Empty;

		}

		public TranslateableEnum<EntryType> AllObjectTypes { get; set; }

		public string GlobalSearchTerm { get; set; }

		public EntryType ObjectType { get; set; }

	}

}