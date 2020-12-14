#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class DataRowViewModel
	{
		public DataRowViewModel(string name, object val, object compareVal = null, bool preserveLineBreaks = false)
		{
			Name = name;
			Val = val;
			CompareVal = compareVal;
			PreserveLineBreaks = preserveLineBreaks;
		}

		public string Name { get; set; }

		public object Val { get; set; }

		public object CompareVal { get; set; }

		public bool PreserveLineBreaks { get; set; }
	}
}