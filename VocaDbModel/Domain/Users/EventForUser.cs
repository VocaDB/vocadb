#nullable disable

using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Domain.Users
{
	public class EventForUser : IEntryWithIntId
	{
		private ReleaseEvent _releaseEvent;
		private User _user;

		public EventForUser() { }

#nullable enable
		public EventForUser(User user, ReleaseEvent releaseEvent, UserEventRelationshipType relationshipType)
		{
			ParamIs.NotNull(() => user);
			ParamIs.NotNull(() => releaseEvent);

			User = user;
			ReleaseEvent = releaseEvent;
			RelationshipType = relationshipType;
		}
#nullable disable

		public virtual int Id { get; set; }

		public virtual UserEventRelationshipType RelationshipType { get; set; }

		public virtual ReleaseEvent ReleaseEvent
		{
			get => _releaseEvent;
			set
			{
				ParamIs.NotNull(() => value);
				_releaseEvent = value;
			}
		}

		public virtual User User
		{
			get => _user;
			set
			{
				ParamIs.NotNull(() => value);
				_user = value;
			}
		}

		public virtual void OnDeleted()
		{
			ReleaseEvent.Users.Remove(this);
			User.Events.Remove(this);
		}
	}

	public enum UserEventRelationshipType
	{
		Interested = 1,
		Attending = 2
	}
}
