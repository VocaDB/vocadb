#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Html
{
	public class MetaTagViewModel
	{
		public MetaTagViewModel(string name, string content)
		{
			Name = name;
			Content = content;
		}

		public string Name { get; set; }

		public string Content { get; set; }
	}
}