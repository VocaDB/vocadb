#nullable disable

using System;

namespace VocaDb.Model.Service.Exceptions
{
	/// <summary>
	/// Exception thrown for duplicate event names.
	/// Event names are unique (case-insensitive and kana-insensitive).
	/// </summary>
	public class DuplicateEventNameException : Exception
	{
		public DuplicateEventNameException(string name, int eventId)
			: base($"Event name already in use: {name} (event Id {eventId})")
		{
			Name = name;
			EventId = eventId;
		}

		public int EventId { get; }

		public string Name { get; }
	}
}
