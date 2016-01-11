using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagCategoryContract {

		public TagCategoryContract() { }

		public TagCategoryContract(string name, ContentLanguagePreference languagePreference, IEnumerable<Tag> tags) {

			ParamIs.NotNull(() => name);
			ParamIs.NotNull(() => tags);

			Name = name;
			Tags = tags.Select(t => new TagBaseContract(t, languagePreference, true)).ToArray();

		}

		public string Name { get; set; }

		public TagBaseContract[] Tags { get; set; }

	}

}
