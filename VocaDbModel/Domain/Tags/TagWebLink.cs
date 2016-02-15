using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.Tags {

	public class TagWebLink : GenericWebLink<Tag> {

		public TagWebLink() {}

		public TagWebLink(Tag entry, WebLinkContract contract) 
			: base(entry, contract) {}

	}

}
