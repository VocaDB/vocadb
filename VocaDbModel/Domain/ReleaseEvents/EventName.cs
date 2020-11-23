using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Model.Domain.ReleaseEvents
{

	public class EventName : EntryName<ReleaseEvent>
	{
		public EventName() { }
		public EventName(ReleaseEvent song, ILocalizedString localizedString) : base(song, localizedString) { }
	}

}
