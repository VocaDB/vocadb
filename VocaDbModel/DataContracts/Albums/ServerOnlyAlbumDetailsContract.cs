#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Albums
{
	[DataContract(Namespace = Schemas.VocaDb, Name = "AlbumDetailsContract")]
	public class ServerOnlyAlbumDetailsContract : AlbumContract
	{
		public ServerOnlyAlbumDetailsContract() { }

		public ServerOnlyAlbumDetailsContract(Album album, ContentLanguagePreference languagePreference, IUserPermissionContext userContext, IAggregatedEntryImageUrlFactory thumbPersister,
			Func<Song, SongVoteRating?> getSongRating = null, Tag discTypeTag = null)
			: base(album, languagePreference)
		{
			ArtistLinks = album.Artists.Select(a => new ArtistForAlbumContract(a, languagePreference)).OrderBy(a => a.Name).ToArray();
			CanEditPersonalDescription = EntryPermissionManager.CanEditPersonalDescription(userContext, album);
			CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, album);
			Description = album.Description;
			Discs = album.Songs.Any(s => s.DiscNumber > 1) ? album.Discs.Select(d => new AlbumDiscPropertiesContract(d)).ToDictionary(a => a.DiscNumber) : new Dictionary<int, AlbumDiscPropertiesContract>(0);
			DiscTypeTypeTag = discTypeTag != null ? new TagBaseContract(discTypeTag, languagePreference) : null;
			OriginalRelease = (album.OriginalRelease != null ? new AlbumReleaseContract(album.OriginalRelease, languagePreference) : null);
			Pictures = album.Pictures.Select(p => new EntryPictureFileContract(p, thumbPersister)).ToArray();
			PVs = album.PVs.Select(p => new PVContract(p)).ToArray();
			Songs = album.Songs
				.OrderBy(s => s.DiscNumber).ThenBy(s => s.TrackNumber)
				.Select(s => new SongInAlbumContract(s, languagePreference, false, rating: getSongRating?.Invoke(s.Song)))
				.ToArray();
			Tags = album.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).OrderByDescending(t => t.Count).ToArray();
			WebLinks = album.WebLinks.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();

			PersonalDescriptionText = album.PersonalDescriptionText;
			var author = album.PersonalDescriptionAuthor;
			PersonalDescriptionAuthor = author != null ? new ArtistForApiContract(author, languagePreference, thumbPersister, ArtistOptionalFields.MainPicture) : null;

			TotalLength = Songs.All(s => s.Song != null && s.Song.LengthSeconds > 0) ? TimeSpan.FromSeconds(Songs.Sum(s => s.Song.LengthSeconds)) : TimeSpan.Zero;
		}

		[DataMember]
		public ServerOnlyAlbumForUserContract AlbumForUser { get; set; }

		[DataMember]
		public ArtistForAlbumContract[] ArtistLinks { get; init; }

		public bool CanEditPersonalDescription { get; init; }

		public bool CanRemoveTagUsages { get; init; }

		[DataMember]
		public int CommentCount { get; init; }

		[DataMember]
		public EnglishTranslatedString Description { get; init; }

		[DataMember]
		public Dictionary<int, AlbumDiscPropertiesContract> Discs { get; init; }

		[DataMember]
		public TagBaseContract DiscTypeTypeTag { get; init; }

		[DataMember]
		public int Hits { get; init; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

		[DataMember]
		public AlbumContract MergedTo { get; set; }

		[DataMember]
		public AlbumReleaseContract OriginalRelease { get; init; }

		[DataMember]
		public string PersonalDescriptionText { get; init; }

		[DataMember]
		public ArtistForApiContract PersonalDescriptionAuthor { get; init; }

		[DataMember]
		public EntryPictureFileContract[] Pictures { get; init; }

		[DataMember]
		public PVContract[] PVs { get; init; }

		[DataMember]
		public SongInAlbumContract[] Songs { get; init; }

		[DataMember]
		public SharedAlbumStatsContract Stats { get; init; }

		[DataMember]
		public TagUsageForApiContract[] Tags { get; init; }

		[DataMember]
		public TimeSpan TotalLength { get; init; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; init; }
	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SharedAlbumStatsContract
	{
		[DataMember]
		public AlbumReviewContract LatestReview { get; init; }

		[DataMember]
		public int LatestReviewRatingScore { get; init; }

		[DataMember]
		public int OwnedCount { get; init; }

		[DataMember]
		public int ReviewCount { get; init; }

		[DataMember]
		public int WishlistCount { get; init; }
	}
}
