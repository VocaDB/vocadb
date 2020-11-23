using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class EntryWithTagUsagesContract : EntryBaseContract
	{
		public EntryWithTagUsagesContract() { }

		public EntryWithTagUsagesContract(IEntryBase entry, IEnumerable<TagUsage> tagUsages, ContentLanguagePreference languagePreference, IUserPermissionContext userContext)
			: base(entry)
		{
			CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, entry);
			TagUsages = tagUsages.Select(u => new TagUsageWithVotesContract(u, languagePreference)).ToArray();
		}

		public bool CanRemoveTagUsages { get; set; }

		public TagUsageWithVotesContract[] TagUsages { get; set; }
	}
}
