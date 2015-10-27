using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagCategoryContract {

		public TagCategoryContract() { }

		public TagCategoryContract(string name, IEnumerable<Tag> tags) {

			ParamIs.NotNull(() => name);
			ParamIs.NotNull(() => tags);

			Name = name;
			Tags = tags.Select(t => new TagBaseContract(t)).ToArray();

		}

		public string Name { get; set; }

		public TagBaseContract[] Tags { get; set; }

	}

}
