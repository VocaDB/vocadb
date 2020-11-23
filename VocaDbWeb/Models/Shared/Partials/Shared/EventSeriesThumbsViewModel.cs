using System.Collections.Generic;
using VocaDb.Model.DataContracts.ReleaseEvents;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class EventSeriesThumbsViewModel
	{
		public EventSeriesThumbsViewModel(IEnumerable<ReleaseEventSeriesContract> events)
		{
			Events = events;
		}

		public IEnumerable<ReleaseEventSeriesContract> Events { get; set; }
	}
}