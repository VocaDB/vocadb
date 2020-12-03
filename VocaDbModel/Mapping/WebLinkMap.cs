using FluentNHibernate.Mapping;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Model.Mapping
{
	public class WebLinkMap<TLink, TEntry> : ClassMap<TLink> where TLink : GenericWebLink<TEntry> where TEntry : class
	{
		public WebLinkMap(bool category = true)
		{
			Cache.ReadWrite();
			Id(m => m.Id);

			if (category)
				Map(m => m.Category).Not.Nullable();

			Map(m => m.Description).Length(512).Not.Nullable();
			Map(m => m.Url).Length(512).Not.Nullable();

			References(m => m.Entry).Column(string.Format("[{0}]", typeof(TEntry).Name)).Not.Nullable();
		}
	}

	public class AlbumWebLinkMap : WebLinkMap<AlbumWebLink, Album> { }

	public class ArtistWebLinkMap : WebLinkMap<ArtistWebLink, Artist> { }

	public class ReleaseEventWebLinkMap : WebLinkMap<ReleaseEventWebLink, ReleaseEvent> { }

	public class ReleaseEventSeriesWebLinkMap : WebLinkMap<ReleaseEventSeriesWebLink, ReleaseEventSeries> { }

	public class SongWebLinkMap : WebLinkMap<SongWebLink, Song> { }

	public class TagWebLinkMap : WebLinkMap<TagWebLink, Tag>
	{
		public TagWebLinkMap() : base(category: false) { }
	}

	public class UserWebLinkMap : WebLinkMap<UserWebLink, User>
	{
		public UserWebLinkMap() : base(category: false) { }
	}

	public class VenueWebLinkMap : WebLinkMap<VenueWebLink, Venue> { }
}
