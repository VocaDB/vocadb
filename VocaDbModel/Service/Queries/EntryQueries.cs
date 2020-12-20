#nullable disable

using System;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.Service.Queries
{
	public class EntryQueries
	{
		public IEntryWithNames Load(EntryRef entryRef, IDatabaseContext ctx) => entryRef.EntryType switch
		{
			EntryType.Album => ctx.Load<Album>(entryRef.Id),
			EntryType.Artist => ctx.Load<Artist>(entryRef.Id),
			EntryType.ReleaseEvent => ctx.Load<ReleaseEvent>(entryRef.Id),
			EntryType.ReleaseEventSeries => ctx.Load<ReleaseEventSeries>(entryRef.Id),
			EntryType.Song => ctx.Load<Song>(entryRef.Id),
			EntryType.Tag => ctx.Load<Tag>(entryRef.Id),
			EntryType.Venue => ctx.Load<Venue>(entryRef.Id),
			_ => throw new ArgumentException("Unsupported entry type: " + entryRef.EntryType),
		};
	}
}
