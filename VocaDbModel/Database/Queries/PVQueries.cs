using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service;

namespace VocaDb.Model.Database.Queries {

	public class PVQueries {

		private readonly IRepository repository;

		public PVQueries(IRepository repository) {
			this.repository = repository;
		}

		public PartialFindResult<PVForSongContract> GetList(string name = null, string author = null,
			PVService? service = null,
			int maxResults = 10, bool getTotalCount = false,
			ContentLanguagePreference lang = ContentLanguagePreference.Default) {

			return repository.HandleQuery(db => {

				var query = db.Query<PVForSong>();

				if (!string.IsNullOrEmpty(name)) {
					query = query.Where(pv => pv.Name == name);
				}

				if (!string.IsNullOrEmpty(author)) {
					query = query.Where(pv => pv.Author == author);
				}

				if (service.HasValue) {
					query = query.Where(pv => pv.Service == service);
				}

				var count = getTotalCount ? query.Count() : 0;

				query = query.Take(maxResults);

				var results = query.Select(p => new PVForSongContract(p, lang)).ToArray();
				return PartialFindResult.Create(results, count);

			});

		}

	}

}
