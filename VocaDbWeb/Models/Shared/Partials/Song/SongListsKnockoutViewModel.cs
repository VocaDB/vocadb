namespace VocaDb.Web.Models.Shared.Partials.Song
{
	public class SongListsKnockoutViewModel
	{
		public SongListsKnockoutViewModel(string binding, bool groupByYear = false)
		{
			Binding = binding;
			GroupByYear = groupByYear;
		}

		public string Binding { get; set; }

		public bool GroupByYear { get; set; }
	}
}