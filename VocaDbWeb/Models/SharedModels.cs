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

	public class LocalizedStringEdit {

		public LocalizedStringEdit() {
			Language = ContentLanguageSelection.Unspecified;
			Value = string.Empty;
		}

		public LocalizedStringEdit(LocalizedStringWithIdContract contract) {

			ParamIs.NotNull(() => contract);

			Id = contract.Id;
			Language = contract.Language;
			Value = contract.Value;

		}

		public int Id { get; set; }

		[Required]
		[Display(Name = "Language")]
		public ContentLanguageSelection Language { get; set; }

		[Required]
		[Display(Name = "Name")]
		public string Value { get; set; }

		public LocalizedStringWithIdContract ToContract() {

			return new LocalizedStringWithIdContract { Id = this.Id, Language = this.Language, Value = this.Value };

		}

	}

}