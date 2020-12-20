#nullable disable

using System.Collections.Generic;
using Microsoft.AspNetCore.Html;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class DataRowListViewModel
	{
		public DataRowListViewModel(string name, IEnumerable<IHtmlContent> rows, IEnumerable<IHtmlContent> compareRows = null)
		{
			Name = name;
			Rows = rows;
			CompareRows = compareRows;
		}

		public string Name { get; set; }

		public IEnumerable<IHtmlContent> Rows { get; set; }

		public IEnumerable<IHtmlContent> CompareRows { get; set; }
	}
}