using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	public class TagCategoryContract {

		public TagCategoryContract() { }

		public TagCategoryContract(string name, IEnumerable<Tag> tags) {

			ParamIs.NotNull(() => name);
			ParamIs.NotNull(() => tags);

			Name = name;
			Tags = tags.Select(t => new TagContract(t)).ToArray();

		}

		public string Name { get; set; }

		public TagContract[] Tags { get; set; }

	}

}
