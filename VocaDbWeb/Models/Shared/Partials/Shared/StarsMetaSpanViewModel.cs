#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class StarsMetaSpanViewModel
	{
		public StarsMetaSpanViewModel(double current, int max)
		{
			Current = current;
			Max = max;
		}

		public double Current { get; set; }

		public int Max { get; set; }
	}
}