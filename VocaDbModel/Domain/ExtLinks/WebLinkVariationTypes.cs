using System;

namespace VocaDb.Model.Domain.ExtLinks {

	[Flags]
	public enum WebLinkVariationTypes {
		/// <summary>
		/// No variations. Only original URL.
		/// </summary>
		Nothing = 0,
		/// <summary>
		/// Include all variations regardless of scheme (no scheme, http, https).
		/// </summary>
		IgnoreScheme = 1,
		/// <summary>
		/// Include variations with and without trailing slash (for example https://www.nicovideo.jp and https://www.nicovideo.jp/)
		/// </summary>
		IgnoreTrailingSlash = 2,
		/// <summary>
		/// Include all variations (scheme and trailing slash)
		/// </summary>
		All = IgnoreScheme | IgnoreTrailingSlash
	}

}
