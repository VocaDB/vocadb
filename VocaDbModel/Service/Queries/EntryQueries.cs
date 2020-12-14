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
		public IEntryWithNames Load(EntryRef entryRef, IDatabaseContext ctx)
		{
			switch (entryRef.EntryType)
			{
				case EntryType.Album:
					return ctx.Load<Album>(entryRef.Id);
				case EntryType.Artist:
					return ctx.Load<Artist>(entryRef.Id);
				case EntryType.ReleaseEvent:
					return ctx.Load<ReleaseEvent>(entryRef.Id);
				case EntryType.ReleaseEventSeries:
					return ctx.Load<ReleaseEventSeries>(entryRef.Id);
				case EntryType.Song:
					return ctx.Load<Song>(entryRef.Id);
				case EntryType.Tag:
					return ctx.Load<Tag>(entryRef.Id);
				case EntryType.Venue:
					return ctx.Load<Venue>(entryRef.Id);
			}

			throw new ArgumentException("Unsupported entry type: " + entryRef.EntryType);
		}
	}
}
