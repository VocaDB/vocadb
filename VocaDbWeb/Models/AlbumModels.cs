using System;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Users;
using VocaDb.Web.Helpers;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Models {

	public class AlbumDetails : IEntryImageInformation {

		private readonly string mime;

		public EntryType EntryType {
			get { return EntryType.Album; }
		}

		public string Mime {
			get { return mime; }
		}

		public AlbumDetails() { }

		public AlbumDetails(AlbumDetailsContract contract) {

			ParamIs.NotNull(() => contract);

			AdditionalNames = contract.AdditionalNames;
			ArtistString = contract.ArtistString;
			CanEdit = EntryPermissionManager.CanEdit(MvcApplication.LoginManager, contract);
			CommentCount = contract.CommentCount;
			CreateDate = contract.CreateDate;
			Description = contract.Description;
			Deleted = contract.Deleted;
			DiscType = contract.DiscType;
			Draft = contract.Status == EntryStatus.Draft;
			Hits = contract.Hits;
			Id = contract.Id;
			LatestComments = contract.LatestComments;
			MergedTo = contract.MergedTo;
			Name = contract.Name;
			OwnedBy = contract.OwnedCount;
			Pictures = contract.Pictures;
			PVs = contract.PVs;
			RatingAverage = contract.RatingAverage;
			RatingCount = contract.RatingCount;
			Songs = contract.Songs.GroupBy(s => s.DiscNumber).ToArray();
			Status = contract.Status;
			Tags = contract.Tags;
			UserHasAlbum = contract.AlbumForUser != null;
			Version = contract.Version;
			WebLinks = contract.WebLinks;
			WishlistedBy = contract.WishlistCount;
			mime = contract.CoverPictureMime;

			if (contract.AlbumForUser != null) {
				AlbumMediaType = contract.AlbumForUser.MediaType;
				AlbumPurchaseStatus = contract.AlbumForUser.PurchaseStatus;
				CollectionRating = contract.AlbumForUser.Rating;
			}

			if (contract.OriginalRelease != null) {
				CatNum = contract.OriginalRelease.CatNum;
				ReleaseEvent = contract.OriginalRelease.EventName;
				ReleaseDate = contract.OriginalRelease.ReleaseDate;
				FullReleaseDate = ReleaseDate.Year.HasValue && ReleaseDate.Month.HasValue && ReleaseDate.Day.HasValue ? (DateTime?)new DateTime(ReleaseDate.Year.Value, ReleaseDate.Month.Value, ReleaseDate.Day.Value) : null;
			}

			var artists = contract.ArtistLinks;

			Bands = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Band)).ToArray();
			Circles = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Circle)).ToArray();
			Labels = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Label)).ToArray();
			Producers = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Producer)).ToArray();
			Vocalists = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Vocalist)).ToArray();
			OtherArtists = artists.Where(a => a.Categories.HasFlag(ArtistCategories.Other) || a.Categories.HasFlag(ArtistCategories.Animator)).ToArray();

			PrimaryPV = PVHelper.PrimaryPV(PVs);

		}

		public string AdditionalNames { get; set; }

		public MediaType AlbumMediaType { get; set; }

		public PurchaseStatus AlbumPurchaseStatus { get; set; }

		public string ArtistString { get; set; }

		public ArtistForAlbumContract[] Bands { get; set; }

		public bool CanEdit { get; set; }

		public string CatNum { get; set; }

		public ArtistForAlbumContract[] Circles { get; set; }

		public int CollectionRating { get; set; }

		public int CommentCount { get; set; }

		public DateTime CreateDate { get; set; }

		public bool Deleted { get; set; }

		public string Description { get; set; }

		public DiscType DiscType { get; set; }

		public bool Draft { get; set; }

		public DateTime? FullReleaseDate { get; set; }

		public int Hits { get; set; }

		public int Id { get; set; }

		public ArtistForAlbumContract[] Labels { get; set; }

		public CommentContract[] LatestComments { get; set; }

		public AlbumContract MergedTo { get; set; }

		public string Name { get; set; }

		public ArtistForAlbumContract[] OtherArtists { get; set; }

		public int OwnedBy { get; set; }

		public EntryPictureFileContract[] Pictures { get; set; }

		public PVContract PrimaryPV { get; set; }

		public ArtistForAlbumContract[] Producers { get; set; }

		public PVContract[] PVs { get; set; }

		public double RatingAverage { get; set; }

		public int RatingCount { get; set; }

		public string ReleaseEvent { get; set; }

		public OptionalDateTimeContract ReleaseDate { get; set; }

		public bool ReleaseDateIsInTheFarFuture {
			get {
				return FullReleaseDate.HasValue && FullReleaseDate.Value > DateTime.Now.AddDays(7);
			}
		}

		public bool ReleaseDateIsInTheNearFuture {
			get {
				return FullReleaseDate.HasValue && FullReleaseDate.Value > DateTime.Now && FullReleaseDate.Value <= DateTime.Now.AddDays(7);
			}
		}

		public bool ReleaseDateIsInThePast {
			get {
				return FullReleaseDate.HasValue && FullReleaseDate.Value <= DateTime.Now;
			}
		}

		public bool ShowProducerRoles {
			get {
				// Show producer roles if more than one producer and other roles besides just composer.
				return Producers.Length > 1 && Producers.Any(p => p.Roles != ArtistRoles.Default && p.Roles != ArtistRoles.Composer);
			}
		}

		public IGrouping<int, SongInAlbumContract>[] Songs { get; set; }

		public EntryStatus Status { get; set; }

		public TagUsageContract[] Tags { get; set; }

		public bool UserHasAlbum { get; set; }

		public int Version { get; set; }

		public ArtistForAlbumContract[] Vocalists { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

		public int WishlistedBy { get; set; }

	}

}