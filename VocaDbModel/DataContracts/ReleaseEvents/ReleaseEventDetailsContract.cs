using System;
using System.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	public class ReleaseEventDetailsContract : ReleaseEventContract {

		public ReleaseEventDetailsContract() {
			WebLinks = new WebLinkContract[0];
		}

		public ReleaseEventDetailsContract(ReleaseEvent releaseEvent, ContentLanguagePreference languagePreference, IUserIconFactory userIconFactory) 
			: base(releaseEvent, true) {

			ParamIs.NotNull(() => releaseEvent);

			SeriesNumber = releaseEvent.SeriesNumber;
			SeriesSuffix = releaseEvent.SeriesSuffix;
			WebLinks = releaseEvent.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

			Albums = releaseEvent.Albums
				.Select(a => new AlbumContract(a, languagePreference))
				.OrderBy(a => a.Name)
				.ToArray();

			Songs = releaseEvent.Songs
				.Select(s => new SongForApiContract(s, languagePreference, SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl))
				.OrderBy(s => s.Name)
				.ToArray();

			UsersAttending = releaseEvent.Users
				.Where(u => u.RelationshipType == UserEventRelationshipType.Attending)
				.Select(u => new UserForApiContract(u.User, userIconFactory, UserOptionalFields.MainPicture))
				.ToArray();

			if (releaseEvent.SongList != null) {
				SongListSongs = releaseEvent.SongList.SongLinks.OrderBy(s => s.Order).Select(s => new SongInListContract(s, languagePreference)).ToArray();
			}

		}

		public AlbumContract[] Albums { get; set; }

		public ReleaseEventSeriesContract[] AllSeries { get; set; }

		public UserEventRelationshipType? EventAssociationType { get; set; }

		public CommentForApiContract[] LatestComments { get; set; }

		public int SeriesNumber { get; set; }

		public string SeriesSuffix { get; set; }

		public SongInListContract[] SongListSongs { get; set; }

		public SongForApiContract[] Songs { get; set; }

		public UserForApiContract[] UsersAttending { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

	}
}
