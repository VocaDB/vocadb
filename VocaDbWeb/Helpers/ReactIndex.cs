using System.Text;
using VocaDb.Web.Code;
using Microsoft.AspNetCore.Mvc;

namespace VocaDb.Web.Helpers;

public class ReactIndex
{
	private readonly IWebHostEnvironment _env;

	public ReactIndex(IWebHostEnvironment env)
	{
		_env = env;
	}

	public ContentResult File(PagePropertiesData properties)
	{
		return new ContentResult() { Content = ToHtml(properties), ContentType = "text/html" };
	}

	private string ToHtml(PagePropertiesData properties)
	{
		// Get the path of the index.html file in the wwwroot folder
		string wwwRootPath = _env.WebRootPath;
		string filePath = Path.Combine(wwwRootPath, "index.html");

		// Read the content of index.html file
		string content = System.IO.File.ReadAllText(filePath);
		content = content.Replace("{{title}}", properties.Title);

		return content;
	}
}