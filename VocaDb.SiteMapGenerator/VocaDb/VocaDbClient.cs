#nullable disable

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NLog;
using VocaDb.SiteMapGenerator.VocaDb.DataContracts;

namespace VocaDb.SiteMapGenerator.VocaDb
{
	public class VocaDbClient
	{
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();
		private readonly string _apiRoot;

		private async Task<T> GetEntries<T>(string apiUrl)
		{
			var uri = new Uri(apiUrl);
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage response;

				try
				{
					response = await client.GetAsync(uri);
					response.EnsureSuccessStatusCode();
				}
				catch (HttpRequestException x)
				{
					_log.Fatal(x, "Unable to get entries from VocaDB API");
					throw;
				}

				try
				{
					var entries = await response.Content.ReadAsAsync<T>();
					return entries;
				}
				catch (UnsupportedMediaTypeException x)
				{
					_log.Fatal(x, "Unable to get entries from VocaDB API");
					throw;
				}
			}
		}

		public VocaDbClient(string apiRoot)
		{
			_apiRoot = apiRoot;
		}

		public async Task<int[]> GetAlbums()
		{
			_log.Info("Getting albums");
			return await GetEntries<int[]>($"{_apiRoot}api/albums/ids");
		}

		public async Task<int[]> GetArtists()
		{
			_log.Info("Getting artists");
			return await GetEntries<int[]>($"{_apiRoot}api/artists/ids");
		}

		public async Task<PartialFindResult<EntryBaseContract>> GetEvents()
		{
			_log.Info("Getting artists");
			return await GetEntries<PartialFindResult<EntryBaseContract>>($"{_apiRoot}api/releaseEvents?maxResults=100000");
		}

		public async Task<int[]> GetSongs()
		{
			_log.Info("Getting songs");
			return await GetEntries<int[]>($"{_apiRoot}api/songs/ids");
		}

		public async Task<PartialFindResult<EntryBaseContract>> GetTags()
		{
			_log.Info("Getting tags");
			return await GetEntries<PartialFindResult<EntryBaseContract>>($"{_apiRoot}api/tags?maxResults=100000");
		}
	}
}
