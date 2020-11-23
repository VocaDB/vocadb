using System.Web;
using System.Web.Mvc;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code.Markdown;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code
{
	public abstract class VocaDbPage<TModel> : WebViewPage<TModel>
	{
		public BrandableStringsManager BrandableStrings => DependencyResolver.Current.GetService<BrandableStringsManager>();

		public VdbConfigManager Config => DependencyResolver.Current.GetService<VdbConfigManager>();

		/// <summary>
		/// Current language preference as integer.
		/// </summary>
		public int LanguagePreferenceInt => (int)UserContext.LanguagePreference;

		public MarkdownParser MarkdownParser => DependencyResolver.Current.GetService<MarkdownParser>();

		public PagePropertiesData PageProperties => PagePropertiesData.Get(ViewBag);

		/// <summary>
		/// Relative path to application root.
		/// 
		/// Cannot be null or empty.
		/// If the application is installed in the root folder, for example http://vocadb.net/, this will be just "/".
		/// For http://server.com/vocadb/ this would be "/vocadb/".
		/// </summary>
		public string RootPath => Url.Content("~/");

		public string DecimalDot(double val)
		{
			return NumberFormatHelper.DecimalDot(val);
		}

		public string ToJS(bool val)
		{
			return val ? "true" : "false";
		}

		public string ToJS(bool? val)
		{
			return val.HasValue ? (val.Value ? "true" : "false") : "null";
		}

		public string ToJS(int? val)
		{
			return val.HasValue ? val.ToString() : "null";
		}

		public IHtmlString ToJS(string str)
		{
			return new MvcHtmlString(JsonHelpers.Serialize(str));
		}

		public IHtmlString ToJS(object obj)
		{
			return new MvcHtmlString(JsonHelpers.Serialize(obj));
		}

		public VocaUrlMapper UrlMapper => new VocaUrlMapper();

		public IUserPermissionContext UserContext => DependencyResolver.Current.GetService<IUserPermissionContext>();
	}

	public abstract class VocaDbPage : WebViewPage
	{
		public BrandableStringsManager BrandableStrings => DependencyResolver.Current.GetService<BrandableStringsManager>();

		public VdbConfigManager Config => DependencyResolver.Current.GetService<VdbConfigManager>();

		public int LanguagePreferenceInt => (int)UserContext.LanguagePreference;

		public PagePropertiesData PageProperties => PagePropertiesData.Get(ViewBag);

		public string RootPath => Url.Content("~/");

		public string ToJS(bool val)
		{
			return val ? "true" : "false";
		}

		public string ToJS(bool? val)
		{
			return val.HasValue ? ToJS(val.Value) : "null";
		}

		public string ToJS(int? val)
		{
			return val.HasValue ? val.ToString() : "null";
		}

		public IHtmlString ToJS(string str)
		{
			return new MvcHtmlString(JsonHelpers.Serialize(str));
		}

		public IHtmlString ToJS(object obj)
		{
			return new MvcHtmlString(JsonHelpers.Serialize(obj));
		}

		public VocaUrlMapper UrlMapper => new VocaUrlMapper();

		public IUserPermissionContext UserContext => DependencyResolver.Current.GetService<IUserPermissionContext>();
	}
}