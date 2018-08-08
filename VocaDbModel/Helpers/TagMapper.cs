using System;
using System.Linq;
using System.Threading.Tasks;
using NHibernate.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Utils.Config;

namespace VocaDb.Model.Helpers {

	public class TagMapper {

		/// <summary>
		/// Tests if a particular tag is redundant for a song and should be skipped from automatic mapping.
		/// </summary>
		public bool TagIsRedundantForSong(SongType songType, int tagId, ISpecialTags specialTags) {

			if (songType == SongType.Cover && tagId == specialTags.Cover)
				return true;

			if (songType == SongType.Remix && tagId == specialTags.Remix)
				return true;

			return false;

		}

		public Tag[] MapTags(IDatabaseContext ctx, string[] nicoTags) {

			// Construct tag mappings (many to many)
			var tagMappings = ctx
				.Query<TagMapping>()
				.Where(t => nicoTags.Contains(t.SourceTag))
				.ToArray()
				.GroupBy(map => map.SourceTag, StringComparer.InvariantCultureIgnoreCase)
				.ToDictionary(grp => grp.Key, grp => grp.Select(map => map.Tag), StringComparer.InvariantCultureIgnoreCase);

			return nicoTags
				.Where(t => tagMappings.ContainsKey(t))
				.SelectMany(t => tagMappings[t])
				.Distinct()
				.ToArray();

		}

		public async Task<Tag[]> MapTagsAsync(IDatabaseContext ctx, string[] nicoTags) {

			// Construct tag mappings (many to many)
			var tagMappingsList = await ctx
				.Query<TagMapping>()
				.Where(t => nicoTags.Contains(t.SourceTag))
				.VdbToListAsync();

			var tagMappings = tagMappingsList
				.GroupBy(map => map.SourceTag, StringComparer.InvariantCultureIgnoreCase)
				.ToDictionary(grp => grp.Key, grp => grp.Select(map => map.Tag), StringComparer.InvariantCultureIgnoreCase);

			return nicoTags
				.Where(t => tagMappings.ContainsKey(t))
				.SelectMany(t => tagMappings[t])
				.Distinct()
				.ToArray();

		}
	}

}
