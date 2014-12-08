using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model;
using VocaDb.Web.Helpers.Support;
using VocaDb.Model.Domain;

namespace VocaDb.Web.Models {

	public class GlobalSearchBoxModel {

		public GlobalSearchBoxModel()
			: this(EntryType.Artist, string.Empty) { }

		public GlobalSearchBoxModel(EntryType? objectType, string searchTerm) {

			AllObjectTypes = new TranslateableEnum<EntryType>(() => global::Resources.EntryTypeNames.ResourceManager, new[] {
				EntryType.Undefined, EntryType.Artist, EntryType.Album, EntryType.Song, EntryType.Tag, EntryType.User
			});
			ObjectType = objectType ?? EntryType.Artist;
			GlobalSearchTerm = searchTerm ?? string.Empty;

		}

		public TranslateableEnum<EntryType> AllObjectTypes { get; set; }

		public string GlobalSearchTerm { get; set; }

		public EntryType ObjectType { get; set; }

	}

}