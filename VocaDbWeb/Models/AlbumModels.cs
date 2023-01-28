#nullable disable

using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models;

public class AlbumDetails : IEntryImageInformation
{
	private readonly string _mime;

	public EntryType EntryType => EntryType.Album;

	public string Mime => _mime;

	ImagePurpose IEntryImageInformation.Purpose => ImagePurpose.Main;

	public AlbumDetails() { }

	public AlbumDetails(AlbumDetailsContract contract, IUserPermissionContext permissionContext, PVHelper pvHelper)
	{
		ParamIs.NotNull(() => contract);

		AdditionalNames = contract.AdditionalNames;
		ArtistString = contract.ArtistString;
		CanEdit = EntryPermissionManager.CanEdit(permissionContext, contract);
		CanEditPersonalDescription = contract.CanEditPersonalDescription;
		CanRemoveTagUsages = contract.CanRemoveTagUsages;
		CommentCount = contract.CommentCount;
		CreateDate = contract.CreateDate;
		Description = contract.Description;
		Deleted = contract.Deleted;
		DiscType = contract.DiscType;
		DiscTypeTypeTag = contract.DiscTypeTypeTag;
		Draft = contract.Status == EntryStatus.Draft;
		Hits = contract.Hits;
		Id = contract.Id;
		LatestComments = contract.LatestComments;
		LatestReview = contract.Stats.LatestReview;
		LatestReviewRatingScore = contract.Stats.LatestReviewRatingScore;
		MergedTo = contract.MergedTo;
		Name = contract.Name;
		OwnedBy = contract.Stats.OwnedCount;
		PersonalDescriptionText = contract.PersonalDescriptionText;
		PersonalDescriptionAuthor = contract.PersonalDescriptionAuthor;
		Pictures = contract.Pictures;
		PVs = contract.PVs;
		RatingAverage = contract.RatingAverage;
		RatingCount = contract.RatingCount;
		ReviewCount = contract.Stats.ReviewCount;
		Status = contract.Status;
		Tags = contract.Tags;
		TotalLength = contract.TotalLength;
		UserHasAlbum = contract.AlbumForUser != null;
		Version = contract.Version;
		WebLinks = contract.WebLinks;
		WishlistedBy = contract.Stats.WishlistCount;
		_mime = contract.CoverPictureMime;

		var songsByDiscs = contract.Songs.GroupBy(s => s.DiscNumber);
		Discs =
			(from songsByDisc in songsByDiscs
			 let dn = songsByDisc.Key
			 select new AlbumDisc(dn, songsByDisc, contract.Discs.ContainsKey(dn) ? contract.Discs[dn] : null))
			.ToArray();

		if (contract.AlbumForUser != null)
		{
			AlbumMediaType = contract.AlbumForUser.MediaType;
			AlbumPurchaseStatus = contract.AlbumForUser.PurchaseStatus;
			CollectionRating = contract.AlbumForUser.Rating;
		}

		if (contract.OriginalRelease != null)
		{
			CatNum = contract.OriginalRelease.CatNum;
			ReleaseEvent = contract.OriginalRelease.ReleaseEvent;
			ReleaseDate = contract.OriginalRelease.ReleaseDate;
			FullReleaseDate = ReleaseDate.Year.HasValue && ReleaseDate.Month.HasValue && ReleaseDate.Day.HasValue ? (DateTime?)new DateTime(ReleaseDate.Year.Value, ReleaseDate.Month.Value, ReleaseDate.Day.Value) : null;
		}

		var artists = contract.ArtistLinks;
		ContentFocus = AlbumHelper.GetContentFocus(DiscType);

		Bands = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Band)).ToArray();
		Circles = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Circle)).ToArray();
		Illustrators = ContentFocus == ContentFocus.Illustration ? artists.Where(a => a.Categories.HasFlag(ArtistCategories.Illustrator)).ToArray() : null;
		Labels = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Label)).ToArray();
		Producers = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Producer)).ToArray();
		Vocalists = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Vocalist)).ToArray();
		Subject = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Subject)).ToArray();
		OtherArtists = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Other)
			|| a.Categories.HasFlag(ArtistCategories.Animator)
			|| (ContentFocus != ContentFocus.Illustration && a.Categories.HasFlag(ArtistCategories.Illustrator))).ToArray();

		PrimaryPV = pvHelper.PrimaryPV(PVs);
	}

	public string AdditionalNames { get; set; }

	public MediaType AlbumMediaType { get; set; }

	public PurchaseStatus AlbumPurchaseStatus { get; set; }

	public string ArtistString { get; set; }

	public ArtistForAlbumContract[] Bands { get; set; }

	public bool CanEdit { get; set; }

	public bool CanEditPersonalDescription { get; set; }

	public bool CanRemoveTagUsages { get; set; }

	public string CatNum { get; set; }

	public ArtistForAlbumContract[] Circles { get; set; }

	public int CollectionRating { get; set; }

	public int CommentCount { get; set; }

	public ContentFocus ContentFocus { get; set; }

	public DateTime CreateDate { get; set; }

	public bool Deleted { get; set; }

	public EnglishTranslatedString Description { get; set; }

	public AlbumDisc[] Discs { get; set; }

	public DiscType DiscType { get; set; }

	public TagBaseContract DiscTypeTypeTag { get; set; }

	public bool Draft { get; set; }

	public DateTime? FullReleaseDate { get; set; }

	public int Hits { get; set; }

	public int Id { get; set; }

	public ArtistForAlbumContract[] Illustrators { get; set; }

	public AlbumDetailsAjax JsonModel => new AlbumDetailsAjax(this);

	public ArtistForAlbumContract[] Labels { get; set; }

	public CommentForApiContract[] LatestComments { get; set; }

	public AlbumReviewContract LatestReview { get; set; }

	public int LatestReviewRatingScore { get; set; }

	public AlbumContract MergedTo { get; set; }

	public string Name { get; set; }

	public ArtistForAlbumContract[] OtherArtists { get; set; }

	public int OwnedBy { get; set; }

	public ArtistForApiContract PersonalDescriptionAuthor { get; set; }

	public string PersonalDescriptionText { get; set; }

	public EntryPictureFileContract[] Pictures { get; set; }

	public PVContract PrimaryPV { get; set; }

	public ArtistForAlbumContract[] Producers { get; set; }

	public PVContract[] PVs { get; set; }

	public double RatingAverage { get; set; }

	public int RatingCount { get; set; }

	public ReleaseEventForApiContract ReleaseEvent { get; set; }

	public OptionalDateTimeContract ReleaseDate { get; set; }

	public bool ReleaseDateIsInTheFarFuture => FullReleaseDate.HasValue && FullReleaseDate.Value > DateTime.Now.AddDays(7);

	public bool ReleaseDateIsInTheNearFuture => FullReleaseDate.HasValue && FullReleaseDate.Value > DateTime.Now && FullReleaseDate.Value <= DateTime.Now.AddDays(7);

	public bool ReleaseDateIsInThePast => FullReleaseDate.HasValue && FullReleaseDate.Value <= DateTime.Now;

	public int ReviewCount { get; set; }

	public bool ShowProducerRoles
	{
		get
		{
			// Show producer roles if more than one producer and other roles besides just composer.
			return Producers.Length > 1 && Producers.Any(p => p.Roles != ArtistRoles.Default && p.Roles != ArtistRoles.Composer);
		}
	}

	public EntryStatus Status { get; set; }

	public ArtistForAlbumContract[] Subject { get; set; }

	public TagUsageForApiContract[] Tags { get; set; }

	public TimeSpan TotalLength { get; set; }

	public bool UserHasAlbum { get; set; }

	public int Version { get; set; }

	public ArtistForAlbumContract[] Vocalists { get; set; }

	public WebLinkContract[] WebLinks { get; set; }

	public int WishlistedBy { get; set; }
}

public class AlbumDisc
{
	public AlbumDisc(int discNumber, IEnumerable<SongInAlbumContract> songs, AlbumDiscPropertiesContract discProperties)
	{
		DiscNumber = discNumber;
		Songs = songs.ToArray();

		IsVideo = discProperties != null && discProperties.MediaType == DiscMediaType.Video;
		Name = discProperties != null ? discProperties.Name : null;
		TotalLength = Songs.All(s => s.Song != null && s.Song.LengthSeconds > 0) ? TimeSpan.FromSeconds(Songs.Sum(s => s.Song.LengthSeconds)) : TimeSpan.Zero;
	}

	public int DiscNumber { get; set; }

	public bool IsVideo { get; set; }

	public TimeSpan TotalLength { get; set; }

	public string Name { get; set; }

	public IEnumerable<SongInAlbumContract> Songs { get; set; }
}

public class AlbumDetailsAjax
{
	public AlbumDetailsAjax(AlbumDetails model)
	{
		Id = model.Id;
		LatestComments = model.LatestComments;
		PersonalDescriptionAuthor = model.PersonalDescriptionAuthor;
		PersonalDescriptionText = model.PersonalDescriptionText;
		TagUsages = model.Tags;
	}

	public int Id { get; set; }

	public CommentForApiContract[] LatestComments { get; set; }

	public ArtistForApiContract PersonalDescriptionAuthor { get; set; }

	public string PersonalDescriptionText { get; set; }

	public TagUsageForApiContract[] TagUsages { get; set; }
}