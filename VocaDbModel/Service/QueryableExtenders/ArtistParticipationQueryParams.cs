using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtenders {

	public class ArtistParticipationQueryParams {

		public ArtistParticipationQueryParams() {}

		public ArtistParticipationQueryParams(EntryIdsCollection artistIds, ArtistAlbumParticipationStatus participation, 
			bool childVoicebanks, bool includeMembers) {

			ArtistIds = artistIds;
			Participation = participation;
			ChildVoicebanks = childVoicebanks;
			IncludeMembers = includeMembers;

		}

		public EntryIdsCollection ArtistIds { get; set; }
		public ArtistAlbumParticipationStatus Participation { get; set; }
		public bool ChildVoicebanks { get; set; }
		public bool IncludeMembers { get; set; }

	}

	public struct ArtistParticipationQueryParams<TEntry, TArtistLink> 
		where TEntry : IEntryWithArtistLinks<TArtistLink> where TArtistLink : IArtistLinkWithRoles {
		
		public ArtistParticipationQueryParams(IQueryable<TEntry> query, ArtistParticipationQueryParams queryParams,
			Func<int, Artist> artistGetter,
			Expression<Func<TEntry, bool>> mainEntriesExpression,
			Expression<Func<TEntry, bool>> collaborationsExpression)
			: this() {

			Query = query;
			ArtistIds = queryParams.ArtistIds;
			Participation = queryParams.Participation;
			ChildVoicebanks = queryParams.ChildVoicebanks;
			IncludeMembers = queryParams.IncludeMembers;
			ArtistGetter = artistGetter;
			MainEntriesExpression = mainEntriesExpression;
			CollaborationsExpression = collaborationsExpression;

		}

		public ArtistParticipationQueryParams(IQueryable<TEntry> query, 
			ArtistParticipationQueryParams queryParams,
			EntryIdsCollection artistIds,
			Func<int, Artist> artistGetter,
			Expression<Func<TEntry, bool>> mainEntriesExpression,
			Expression<Func<TEntry, bool>> collaborationsExpression)
			: this() {

			Query = query;
			ArtistIds = artistIds;
			Participation = queryParams.Participation;
			ChildVoicebanks = queryParams.ChildVoicebanks;
			IncludeMembers = queryParams.IncludeMembers;
			ArtistGetter = artistGetter;
			MainEntriesExpression = mainEntriesExpression;
			CollaborationsExpression = collaborationsExpression;

		}

		public IQueryable<TEntry> Query { get; } 
		public EntryIdsCollection ArtistIds { get; } 
		public ArtistAlbumParticipationStatus Participation { get; }
		public bool ChildVoicebanks { get; }
		public bool IncludeMembers { get; }
		public Func<int, Artist> ArtistGetter { get; }
		public Expression<Func<TEntry, bool>> MainEntriesExpression { get; }
		public Expression<Func<TEntry, bool>> CollaborationsExpression { get; }

	}

}