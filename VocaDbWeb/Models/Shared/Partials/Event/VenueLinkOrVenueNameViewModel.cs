using VocaDb.Model.DataContracts.ReleaseEvents;

namespace VocaDb.Web.Models.Shared.Partials.Event
{

	public class VenueLinkOrVenueNameViewModel
	{

		public VenueLinkOrVenueNameViewModel(ReleaseEventForApiContract @event)
		{
			Event = @event;
		}

		public ReleaseEventForApiContract Event { get; set; }

	}

}