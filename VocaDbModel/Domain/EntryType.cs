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

		Album = 1,

		Artist = 2,

		DiscussionTopic = 4,

		PV = 8,

		ReleaseEvent = 16,

		ReleaseEventSeries = 32,

		Song = 64,

		SongList = 128,

		Tag = 256,

		User = 512,

		Venue = 1024
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
		Venue = EntryType.Venue
	}
}
