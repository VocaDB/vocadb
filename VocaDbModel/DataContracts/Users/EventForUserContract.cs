#nullable disable

using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	public class EventForUserContract
	{
		public static EventForUserContract CreateForUser(EventForUser link, ContentLanguagePreference languagePreference, ReleaseEventOptionalFields releaseEventFields, IAggregatedEntryImageUrlFactory entryThumbPersister)
		{
			return new EventForUserContract
			{
				RelationshipType = link.RelationshipType,
				ReleaseEvent = new ReleaseEventForApiContract(link.ReleaseEvent, languagePreference, releaseEventFields, entryThumbPersister)
			};
		}

		public UserEventRelationshipType RelationshipType { get; init; }

		public ReleaseEventForApiContract ReleaseEvent { get; init; }

		public UserForApiContract User { get; init; }
	}
}
