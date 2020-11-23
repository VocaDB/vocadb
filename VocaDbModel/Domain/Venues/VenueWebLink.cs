using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.Venues
{

	public class VenueWebLink : GenericWebLink<Venue>
	{

		public VenueWebLink() { }

		public VenueWebLink(Venue venue, string description, string url, WebLinkCategory category)
			: base(venue, description, url, category) { }

	}

}
