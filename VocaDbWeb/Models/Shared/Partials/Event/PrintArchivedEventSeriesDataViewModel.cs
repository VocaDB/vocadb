using VocaDb.Model.DataContracts.ReleaseEvents;

namespace VocaDb.Web.Models.Shared.Partials.Event
{

	public class PrintArchivedEventSeriesDataViewModel
	{

		public PrintArchivedEventSeriesDataViewModel(ComparedEventSeriesContract comparedSeries)
		{
			ComparedSeries = comparedSeries;
		}

		public ComparedEventSeriesContract ComparedSeries { get; set; }

	}

}