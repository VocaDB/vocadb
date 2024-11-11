using System.Text;
using VocaDb.Web.Code;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Utils.Config;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Helpers;

public class ReactIndex
{
	private readonly IWebHostEnvironment _env;
	private readonly BrandableStringsManager _brandableStrings;
	private readonly VdbConfigManager _config;
	private readonly IUserPermissionContext _permissionContext;
    private readonly string _indexHtmlContent;

    public ReactIndex(IWebHostEnvironment env, BrandableStringsManager brandableStrings, VdbConfigManager config, IUserPermissionContext permissionContext)
    {
        _env = env;
        _brandableStrings = brandableStrings;
        _config = config;
        _permissionContext = permissionContext;

        // Read the content of index.html file once on startup
        string wwwRootPath = _env.WebRootPath;
        string filePath = Path.Combine(wwwRootPath, "index.html");
        _indexHtmlContent = System.IO.File.ReadAllText(filePath);
    }

	public ContentResult File(PagePropertiesData properties)
	{
		return new ContentResult() { Content = ToHtml(properties), ContentType = "text/html" };
	}

	private string NewOpenGraphMetaTag(string name, string? content)
	{
		if (string.IsNullOrEmpty(content))
		{
			return "";
		}
		return $"<meta name=\"{name}\" content=\"{content}\" />";
	}

	private string CreateOpenGraphMetaTags(PagePropertiesData properties)
	{
		var tags = new StringBuilder();

		tags.Append(NewOpenGraphMetaTag("og:url", properties.CanonicalUrl));
		tags.Append(NewOpenGraphMetaTag("og:title", properties.OpenGraph.Title));
		tags.Append(NewOpenGraphMetaTag("og:description", properties.OpenGraph.Description));
		tags.Append(NewOpenGraphMetaTag("og:type", properties.OpenGraph.Type));

		if (properties.OpenGraph.ShowTwitterCard && !string.IsNullOrEmpty(properties.OpenGraph.Title) && !string.IsNullOrEmpty(properties.OpenGraph.Description))
		{
			tags.Append("<meta name=\"twitter:card\" content=\"summary\" />")
				.AppendFormat("<meta name=\"twitter:site\" content=\"@{0}\" />", _config.SiteSettings.TwitterAccountName)
				.AppendFormat("<meta name=\"twitter:title\" content=\"{0}\" />", properties.OpenGraph.Title)
				.AppendFormat("<meta name=\"twitter:description\" content=\"{0}\" />", properties.OpenGraph.Description);

			if (!string.IsNullOrEmpty(properties.OpenGraph.Image))
			{
				tags.AppendFormat("<meta name=\"twitter:image\" content=\"{0}\" />", properties.OpenGraph.Image);
			}
		}

		return tags.ToString();
	}

	private string ToHtml(PagePropertiesData properties)
	{
		var content = _indexHtmlContent
			.Replace("{{title}}",
				(!string.IsNullOrEmpty(properties.PageTitle) ? properties.PageTitle + " - " : "") +
				_brandableStrings.SiteTitle)
			.Replace("{{summarizedDescription}}", properties.SummarizedDescription)
			.Replace("{{keywords}}", _brandableStrings.Layout.Keywords)
			.Replace("{{ogImage}}", properties.OpenGraph.Image)
			.Replace("{{siteName}}", _brandableStrings.SiteName)
			.Replace("{{osPath}}", _config.SiteSettings.OpenSearchPath)
			.Replace("{{favicon}}",
				!string.IsNullOrEmpty(_config.Assets.FavIconUrl) ? _config.Assets.FavIconUrl : "/Content/favicon.ico");

		// TODO: Make this configurable
		// Activates the Crowdin In-Context translation feature
		if (_permissionContext.LoggedUser?.Language == "quc")
		{
			content = content.Replace("<!--{incontextloc}", "")
				.Replace("{incontextloc}-->", "");
		}
		
		// TODO: Make this configurable
		// Activates umami tracking
		if (_env.IsProduction())
		{
			content = content.Replace("<!--{trackingcode}", "")
				.Replace("{trackingcode}-->", "")
				.Replace("{{dataWebsiteId}}",
					_brandableStrings.SiteName == "VocaDB" ? "981e1817-1212-4ff5-ac3a-67fceaa9912d" :
					_brandableStrings.SiteName == "UtaiteDB" ? "9ee1b267-bdbe-4949-b5cb-ca4e112adc8c" :
					"57cf43f5-3fbd-4ac1-9c7b-0e95bc8a098c");
		}

		var preInd = content.IndexOf("</head>", StringComparison.Ordinal);

		if (!string.IsNullOrEmpty(properties.Robots))
		{
			content = content.Insert(preInd, "<meta name=\"robots\" content=" + properties.Robots + "/>");
		}

		if (!string.IsNullOrEmpty(properties.CanonicalUrl))
		{
			content = content.Insert(preInd, $"<link rel=\"canonical\" href=\"{properties.CanonicalUrl}\" />");
		}

		content = content.Insert(preInd, CreateOpenGraphMetaTags(properties));

		return content;
	}
}