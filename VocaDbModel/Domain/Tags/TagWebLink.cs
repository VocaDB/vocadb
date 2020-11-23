using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.Tags
{

	public class TagWebLink : GenericWebLink<Tag>
	{

		public TagWebLink() { }

		public TagWebLink(Tag tag, WebLinkContract contract)
			: base(tag, contract) { }

		public TagWebLink(Tag tag, string description, string url)
			: base(tag, description, url, WebLinkCategory.Other) { }

	}

}
