#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class ServerOnlyEntryWithTagUsagesContract : EntryBaseContract
	{
		public ServerOnlyEntryWithTagUsagesContract() { }

		public ServerOnlyEntryWithTagUsagesContract(IEntryBase entry, IEnumerable<TagUsage> tagUsages, ContentLanguagePreference languagePreference, IUserPermissionContext userContext, IUserIconFactory userIconFactory)
			: base(entry)
		{
			CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, entry);
			TagUsages = tagUsages.Select(u => new ServerOnlyTagUsageWithVotesContract(u, languagePreference, userIconFactory)).ToArray();
		}

		public bool CanRemoveTagUsages { get; init; }

		public ServerOnlyTagUsageWithVotesContract[] TagUsages { get; init; }
	}
}
