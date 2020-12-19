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
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
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
					s_log.Fatal(x, "Unable to get entries from VocaDB API");
					throw;
				}

				try
				{
					var entries = await response.Content.ReadAsAsync<T>();
					return entries;
				}
				catch (UnsupportedMediaTypeException x)
				{
					s_log.Fatal(x, "Unable to get entries from VocaDB API");
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
			s_log.Info("Getting albums");
			return await GetEntries<int[]>($"{_apiRoot}api/albums/ids");
		}

		public async Task<int[]> GetArtists()
		{
			s_log.Info("Getting artists");
			return await GetEntries<int[]>($"{_apiRoot}api/artists/ids");
		}

		public async Task<PartialFindResult<EntryBaseContract>> GetEvents()
		{
			s_log.Info("Getting artists");
			return await GetEntries<PartialFindResult<EntryBaseContract>>($"{_apiRoot}api/releaseEvents?maxResults=100000");
		}

		public async Task<int[]> GetSongs()
		{
			s_log.Info("Getting songs");
			return await GetEntries<int[]>($"{_apiRoot}api/songs/ids");
		}

		public async Task<PartialFindResult<EntryBaseContract>> GetTags()
		{
			s_log.Info("Getting tags");
			return await GetEntries<PartialFindResult<EntryBaseContract>>($"{_apiRoot}api/tags?maxResults=100000");
		}
	}
}
