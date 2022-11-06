using System.Web;
using Microsoft.AspNetCore.Html;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Helpers;

public static class JsonHelpers
{
	/// <summary>
	/// <seealso href="https://github.com/VocaDB/vocadb/pull/736"/>
	/// </summary>
	public static IHtmlContent ToJS(object? value, bool lowerCase = true, bool dateTimeConverter = false)
	{
		return new HtmlString($@"JSON.parse(""{HttpUtility.JavaScriptStringEncode(JsonHelper.Serialize(value, lowerCase, dateTimeConverter))}"")");
	}
}