#nullable disable

using VocaDb.Model.DataContracts.ReleaseEvents;

namespace VocaDb.Web.Models.Shared.Partials.Event
{
	public class PrintArchivedEventDataViewModel
	{
		public PrintArchivedEventDataViewModel(ComparedEventsContract comparedEvents)
		{
			ComparedEvents = comparedEvents;
		}

		public ComparedEventsContract ComparedEvents { get; set; }
	}
}