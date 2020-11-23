using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public class ReleaseEventWebLink : GenericWebLink<ReleaseEvent>
	{
		public ReleaseEventWebLink() { }

		public ReleaseEventWebLink(ReleaseEvent releaseEvent, string description, string url, WebLinkCategory category)
			: base(releaseEvent, description, url, category) { }
	}
}
