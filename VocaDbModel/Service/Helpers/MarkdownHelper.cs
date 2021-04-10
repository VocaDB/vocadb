namespace VocaDb.Model.Service.Helpers
{
	public class MarkdownHelper
	{
		public static string CreateMarkdownLink(string? url, string? name)
		{
			return $"[{name}]({url})";
		}
	}
}