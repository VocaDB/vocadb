using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.VideoServices
{
	public interface IPVParser
	{
		/// <summary>
		/// Parses PV by URL.
		/// </summary>
		/// <param name="url">URL to be parsed. Cannot be null or empty.</param>
		/// <param name="getTitle">Whether to load metadata such as title and video author.</param>
		/// <param name="permissionContext">Permission context. Can be null (if the user is not logged in).</param>
		/// <returns>Result of PV parsing. Cannot be null.</returns>
		Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle, IUserPermissionContext permissionContext);

		/// <summary>
		/// Parses PV by URL.
		/// </summary>
		/// <param name="url">URL to be parsed. Cannot be null or empty.</param>
		/// <param name="getTitle">Whether to load metadata such as title and video author.</param>
		/// <param name="permissionContext">Permission context. Can be null (if the user is not logged in).</param>
		/// <returns>Result of PV parsing. Cannot be null.</returns>
		Task<VideoUrlParseResult[]> ParseByUrlsAsync(IEnumerable<string> urls, bool getTitle, IUserPermissionContext permissionContext);
	}

	public class PVParser : IPVParser
	{
		public Task<VideoUrlParseResult> ParseByUrlAsync(string url, bool getTitle, IUserPermissionContext permissionContext) => VideoServiceHelper.ParseByUrlAsync(url, getTitle, permissionContext);

		public Task<VideoUrlParseResult[]> ParseByUrlsAsync(IEnumerable<string> urls, bool getTitle, IUserPermissionContext permissionContext)
		{
			return Task.WhenAll(urls.Select(url => ParseByUrlAsync(url, getTitle, permissionContext)));
		}
	}
}
