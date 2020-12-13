using System.Collections.Generic;
using System.Web;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class DataRowListViewModel
	{
		public DataRowListViewModel(string name, IEnumerable<IHtmlString> rows, IEnumerable<IHtmlString> compareRows = null)
		{
			Name = name;
			Rows = rows;
			CompareRows = compareRows;
		}

		public string Name { get; set; }

		public IEnumerable<IHtmlString> Rows { get; set; }

		public IEnumerable<IHtmlString> CompareRows { get; set; }
	}
}