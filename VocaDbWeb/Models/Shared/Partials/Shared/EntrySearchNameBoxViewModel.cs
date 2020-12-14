#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class EntrySearchNameBoxViewModel
	{
		public EntrySearchNameBoxViewModel(string id, string cls = "input-xlarge")
		{
			Id = id;
			Cls = cls;
		}

		public string Id { get; set; }

		public string Cls { get; set; }
	}
}