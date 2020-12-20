#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Code
{
	/// <summary>
	/// Common properties for a page, to be used by the layout page.
	/// </summary>
	public class PagePropertiesData
	{
		public const string Robots_Noindex_Follow = "noindex,follow";

		public const string Robots_Noindex_Nofollow = "noindex,nofollow";

		public static PagePropertiesData Get(dynamic viewBag)
		{
			return viewBag.PageProperties ?? (viewBag.PageProperties = new PagePropertiesData(viewBag));
		}

		private readonly dynamic _viewBag;

		public PagePropertiesData(dynamic viewBag)
		{
			AddMainScripts = true;
			GlobalSearchType = EntryType.Undefined;
			_viewBag = viewBag;
			OpenGraph = new OpenGraphModel(this);
		}

		/// <summary>
		/// Include main scripts (on most pages except the front page).
		/// </summary>
		public bool AddMainScripts { get; set; }

		public string CanonicalUrl { get; set; }

		/// <summary>
		/// Description meta field, also the default value for og:description.
		/// This should be plain text (no HTML or Markdown).
		/// </summary>
		public string Description { get; set; }

		public EntryType GlobalSearchType { get; set; }

		public OpenGraphModel OpenGraph { get; }

		/// <summary>
		/// Page title is what appears in the browser title bar.
		/// By default this is the same as Title.
		/// </summary>
		public string PageTitle
		{
			get
			{
				if (!string.IsNullOrEmpty(ViewBag.PageTitle))
					return ViewBag.PageTitle;

				return (string)ViewBag.Title ?? string.Empty;
			}
			set => ViewBag.PageTitle = value;
		}

		public string Robots { get; set; }

		/// <summary>
		/// Short page description 
		/// </summary>
		public string SummarizedDescription
		{
			get
			{
				if (string.IsNullOrEmpty(Description))
					return string.Empty;

				// TODO (PERF): this should be cached. Actually, the original description isn't even used.
				return Description
					.Trim()
					.Summarize(30, 300);
			}
		}

		/// <summary>
		/// Subtitle appears next to the main Title.
		/// </summary>
		public string Subtitle
		{
			get => ViewBag.Subtitle;
			set => ViewBag.Subtitle = value;
		}

		/// <summary>
		/// Title is what appears at the top of the page.
		/// </summary>
		public string Title
		{
			get => ViewBag.Title;
			set => ViewBag.Title = value;
		}

		public dynamic ViewBag => _viewBag;
	}

	public class OpenGraphModel
	{
		private string _description;
		private string _image;
		private readonly PagePropertiesData _pageProperties;
		private string _title;

		public OpenGraphModel(PagePropertiesData pageProperties)
		{
			_pageProperties = pageProperties;
		}

		/// <summary>
		/// og:description meta field value. Defaults to summarized description meta field.
		/// </summary>
		public string Description
		{
			get => !string.IsNullOrEmpty(_description) ? _description : _pageProperties.SummarizedDescription;
			set => _description = value;
		}

		/// <summary>
		/// og:image meta field value. Defaults to main image set for the page.
		/// </summary>
		public string Image
		{
			get => !string.IsNullOrEmpty(_image) ? _image : _pageProperties.ViewBag.Banner;
			set => _image = value;
		}

		public bool ShowTwitterCard { get; set; }

		/// <summary>
		/// og:title meta field value. Defaults to page title.
		/// </summary>
		public string Title
		{
			get => !string.IsNullOrEmpty(_title) ? _title : _pageProperties.Title;
			set => _title = value;
		}

		/// <summary>
		/// og:type meta field value.
		/// </summary>
		public string Type { get; set; }
	}

	public static class OpenGraphTypes
	{
		public const string Album = "music.album";
		public const string Song = "music.song";
	}
}