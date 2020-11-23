namespace VocaDb.Model.Service.Helpers
{

	public class MarkdownHelper
	{

		public static string CreateMarkdownLink(string url, string name)
		{
			return string.Format("[{0}]({1})", name, url);
		}

	}

}