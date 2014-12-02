using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Formatting;
using System.Resources;
using System.Web.Http;
using System.Web.Http.Controllers;
using Newtonsoft.Json.Serialization;
using WebApi.OutputCache.V2;

namespace VocaDb.Web.Controllers.Api {

	/// <summary>
	/// Loads localized string resources.
	/// </summary>
	[RoutePrefix("api/resources")]
	[DefaultCasingConfig]
	public class ResourcesApiController : ApiController {

		class DefaultCasingConfig : Attribute, IControllerConfiguration {

			public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor) {

				controllerSettings.Formatters.Clear();
				controllerSettings.Formatters.Add(new JsonMediaTypeFormatter());
				controllerSettings.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();

			}

		}

		private const int cacheDuration = 86400; // 24h

		private readonly Dictionary<string, ResourceManager> allSets = new Dictionary<string, ResourceManager> {
			{ "albumMediaTypeNames", global::Resources.AlbumMediaTypeNames.ResourceManager },
			{ "albumSortRuleNames", global::Resources.AlbumSortRuleNames.ResourceManager },
			{ "artistSortRuleNames", global::Resources.ArtistSortRuleNames.ResourceManager },
			{ "artistTypeNames", Model.Resources.ArtistTypeNames.ResourceManager },
			{ "discTypeNames", Model.Resources.Albums.DiscTypeNames.ResourceManager },
			{ "entryTypeNames", global::Resources.EntryTypeNames.ResourceManager },
			{ "songSortRuleNames", global::Resources.SongSortRuleNames.ResourceManager },
			{ "songTypeNames", Model.Resources.Songs.SongTypeNames.ResourceManager },
		};

		private Dictionary<string, string> GetResources(string setName, CultureInfo culture) {

			ResourceManager resourceManager;
			if (!allSets.TryGetValue(setName, out resourceManager))
				return new Dictionary<string, string>();

			var set = resourceManager.GetResourceSet(culture, true, true);
			return set.Cast<DictionaryEntry>().ToDictionary(s => (string)s.Key, s => (string)s.Value);

		}

		/// <summary>
		/// Gets a number of resource sets for a specific culture.
		/// </summary>
		/// <param name="cultureCode">Culture code, for example "en-US" or "fi-FI".</param>
		/// <param name="setNames">Names of resource sets to be returned. More than one value can be specified. For example "artistTypeNames"</param>
		/// <returns>Resource sets. Cannot be null.</returns>
		[Route("{cultureCode}")]
		[CacheOutput(ClientTimeSpan = cacheDuration, ServerTimeSpan = cacheDuration)]
		public Dictionary<string, Dictionary<string, string>> GetList(string cultureCode, [FromUri] string[] setNames) {

			if (setNames == null || !setNames.Any()) {
				return new Dictionary<string, Dictionary<string, string>>();
			}

			CultureInfo culture = null;
			if (!string.IsNullOrEmpty(cultureCode)) {
				try {
					culture = CultureInfo.GetCultureInfo(cultureCode);
				} catch (CultureNotFoundException) {}
			}

			if (culture == null)
				culture = CultureInfo.GetCultureInfo("en-US");

			var sets = setNames.ToDictionary(s => s, s => GetResources(s, culture));
			return sets;

		} 

	}

}