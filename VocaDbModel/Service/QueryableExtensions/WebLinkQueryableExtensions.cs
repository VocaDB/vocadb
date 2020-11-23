using System.Linq;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.QueryableExtensions
{
	/// <summary>
	/// Extensions for <see cref="IQueryable{WebLink}"/>.
	/// Also see <see cref="EntryWithLinksQueryableExtensions"/>.
	/// </summary>
	public static class WebLinkQueryableExtensions
	{
		/// <summary>
		/// Matches weblink by URL, including all scheme variations (no scheme, http, https).
		/// </summary>
		/// <remarks>
		/// If link is https://www.nicovideo.jp/user/9979822, this matches
		/// www.nicovideo.jp/user/9979822 (no scheme)
		/// https://www.nicovideo.jp/user/9979822 (https)
		/// http://www.nicovideo.jp/user/9979822 (http)
		/// </remarks>
		public static IQueryable<TLink> WhereUrlIs<TLink>(this IQueryable<TLink> query, string url, WebLinkVariationTypes variationTypes)
			where TLink : class, IWebLink
		{
			var variations = WebLinkVariationsFactory.GetWebLinkVariations(url, variationTypes);
			return query.Where(w => variations.Contains(w.Url));
		}
	}
}
