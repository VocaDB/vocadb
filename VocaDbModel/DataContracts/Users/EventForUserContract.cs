using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users {

	public class EventForUserContract {

		public static EventForUserContract CreateForUser(EventForUser link, ReleaseEventOptionalFields releaseEventFields, IEntryThumbPersister entryThumbPersister) {

			return new EventForUserContract {
				RelationshipType = link.RelationshipType,
				ReleaseEvent = new ReleaseEventForApiContract(link.ReleaseEvent, releaseEventFields, entryThumbPersister, true)
			};

		}

		public UserEventRelationshipType RelationshipType { get; set; }

		public ReleaseEventForApiContract ReleaseEvent { get; set; }

		public UserForApiContract User { get; set; }

	}
}
