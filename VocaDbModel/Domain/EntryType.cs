#nullable disable

using System;

namespace VocaDb.Model.Domain
{
	/// <summary>
	/// Do not change the numeric values here.
	/// </summary>
	public enum EntryType
	{
		Undefined = 0,

		Album = 1 << 0,

		Artist = 1 << 1,

		DiscussionTopic = 1 << 2,

		PV = 1 << 3,

		ReleaseEvent = 1 << 4,

		ReleaseEventSeries = 1 << 5,

		Song = 1 << 6,

		SongList = 1 << 7,

		Tag = 1 << 8,

		User = 1 << 9,

		Venue = 1 << 10,
	}

	[Flags]
	public enum EntryTypes
	{
		Nothing = 0,
		Album = EntryType.Album,
		Artist = EntryType.Artist,
		DiscussionTopic = EntryType.DiscussionTopic,
		PV = EntryType.PV,
		ReleaseEvent = EntryType.ReleaseEvent,
		ReleaseEventSeries = EntryType.ReleaseEventSeries,
		Song = EntryType.Song,
		SongList = EntryType.SongList,
		Tag = EntryType.Tag,
		User = EntryType.User,
		Venue = EntryType.Venue,
	}
}
