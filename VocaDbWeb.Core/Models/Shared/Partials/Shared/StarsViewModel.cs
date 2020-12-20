#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class StarsViewModel
	{
		public StarsViewModel(int current, int max)
		{
			Current = current;
			Max = max;
		}

		public int Current { get; set; }

		public int Max { get; set; }
	}
}