namespace VocaDb.Web.Models.Shared.Partials.Shared
{

	public class ValidationSummaryPanelViewModel
	{

		public ValidationSummaryPanelViewModel(string message)
		{
			Message = message;
		}

		public string Message { get; set; }

	}

}