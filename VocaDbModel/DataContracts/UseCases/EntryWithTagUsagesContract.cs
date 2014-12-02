using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.UseCases {

	public class EntryWithTagUsagesContract : EntryBaseContract {

		public EntryWithTagUsagesContract() { }

		public EntryWithTagUsagesContract(IEntryBase entry, IEnumerable<TagUsage> tagUsages)
			: base(entry) {

			TagUsages = tagUsages.Select(u => new TagUsageWithVotesContract(u)).ToArray();

		}

		public TagUsageWithVotesContract[] TagUsages { get; set; }

	}

}
