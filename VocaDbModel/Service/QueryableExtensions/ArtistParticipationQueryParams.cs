using System;
using System.Linq;
using System.Linq.Expressions;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.AlbumSearch;

namespace VocaDb.Model.Service.QueryableExtensions
{
	/// <summary>
	/// Parameters for filtering content (usually songs or albums) based on artists' <see cref=""ArtistAlbumParticipationStatus/>.
	/// </summary>
	public class ArtistParticipationQueryParams
	{
		public ArtistParticipationQueryParams() { }

		public ArtistParticipationQueryParams(EntryIdsCollection artistIds, ArtistAlbumParticipationStatus participation,
			bool childVoicebanks, bool includeMembers)
		{
			ArtistIds = artistIds;
			Participation = participation;
			ChildVoicebanks = childVoicebanks;
			IncludeMembers = includeMembers;
		}

		/// <summary>
		/// List of artist IDs for which to get the content.
		/// </summary>
		public EntryIdsCollection ArtistIds { get; set; }
		public ArtistAlbumParticipationStatus Participation { get; set; }
		/// <summary>
		/// For voicebanks, include child voicebanks.
		/// </summary>
		public bool ChildVoicebanks { get; set; }
		/// <summary>
		/// For groups, include members.
		/// </summary>
		public bool IncludeMembers { get; set; }
	}

	public readonly struct ArtistParticipationQueryParams<TEntry, TArtistLink>
		where TEntry : IEntryWithArtistLinks<TArtistLink>
		where TArtistLink : IArtistLinkWithRoles
	{
		public ArtistParticipationQueryParams(IQueryable<TEntry> query, ArtistParticipationQueryParams queryParams,
			IEntityLoader<Artist> artistGetter,
			Expression<Func<TEntry, bool>> mainEntriesExpression,
			Expression<Func<TEntry, bool>> collaborationsExpression)
			: this()
		{
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
			IEntityLoader<Artist> artistGetter,
			Expression<Func<TEntry, bool>> mainEntriesExpression,
			Expression<Func<TEntry, bool>> collaborationsExpression)
			: this()
		{
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
		public IEntityLoader<Artist> ArtistGetter { get; }
		public Expression<Func<TEntry, bool>> MainEntriesExpression { get; }
		public Expression<Func<TEntry, bool>> CollaborationsExpression { get; }
	}
}