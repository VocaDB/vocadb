using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.VideoServices;
using VocaDb.Web.Helpers;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Web.Models {

	public class SongDetails {

		public SongDetails(SongDetailsContract contract, IUserPermissionContext userContext) {

			ParamIs.NotNull(() => contract);

			Contract = contract;
			AdditionalNames = contract.AdditionalNames;
			Albums = contract.Albums;
			AlternateVersions = contract.AlternateVersions.Where(a => a.SongType != SongType.Original).ToArray();
			ArtistString = contract.ArtistString;
			CanEdit = EntryPermissionManager.CanEdit(userContext, contract.Song);
			CommentCount = contract.CommentCount;
			CreateDate = contract.CreateDate;
			DefaultLanguageSelection = contract.TranslatedName.DefaultLanguage;
			Deleted = contract.Deleted;
			Draft = contract.Song.Status == EntryStatus.Draft;
			FavoritedTimes = contract.Song.FavoritedTimes;
			Hits = contract.Hits;
			Id = contract.Song.Id;
			IsFavorited = contract.UserRating != SongVoteRating.Nothing;
			Json = JsonHelpers.Serialize(new SongDetailsAjax(this, contract.PreferredLyrics));
			LatestComments = contract.LatestComments;
			Length = contract.Song.LengthSeconds;
			LikedTimes = contract.LikeCount;
			ListCount = contract.ListCount;
			Lyrics = contract.LyricsFromParents;
			MergedTo = contract.MergedTo;
			Name = contract.Song.Name;
			NicoId = contract.Song.NicoId;
			Notes = contract.Notes;
			OriginalVersion = (contract.Song.SongType != SongType.Original ? contract.OriginalVersion : null);
			Pools = contract.Pools;
			PublishDate = contract.Song.PublishDate;
			RatingScore = contract.Song.RatingScore;
			ReleaseEvent = contract.ReleaseEvent;
			SongType = contract.Song.SongType;
			Status = contract.Song.Status;
			Suggestions = contract.Suggestions;
			Tags = contract.Tags;
			UserRating = contract.UserRating;
			WebLinks = contract.WebLinks.ToList();

			Animators = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Animator)).ToArray();
			Bands = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Band)).ToArray();
			Performers = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Vocalist)).ToArray();
			Producers = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Producer)).ToArray();
			OtherArtists = contract.Artists.Where(a => a.Categories.HasFlag(ArtistCategories.Circle)  
				|| a.Categories.HasFlag(ArtistCategories.Label) 
				|| a.Categories.HasFlag(ArtistCategories.Other)).ToArray();

			var pvs = contract.PVs;

			OriginalPVs = pvs.Where(p => p.PVType == PVType.Original).ToArray();
			OtherPVs = pvs.Where(p => p.PVType != PVType.Original).ToArray();
			PrimaryPV = PVHelper.PrimaryPV(pvs);
			ThumbUrl = VideoServiceHelper.GetThumbUrlPreferNotNico(pvs);
			ThumbUrlMaxSize = VideoServiceHelper.GetMaxSizeThumbUrl(pvs) ?? ThumbUrl;

			if (PrimaryPV == null && !string.IsNullOrEmpty(NicoId))
				PrimaryPV = new PVContract { PVId = NicoId, Service = PVService.NicoNicoDouga };

			if (pvs.All(p => p.Service != PVService.Youtube)) {

				var nicoPV = VideoServiceHelper.PrimaryPV(pvs, PVService.NicoNicoDouga);
				var query = HttpUtility.UrlEncode((nicoPV != null && !string.IsNullOrEmpty(nicoPV.Name))
					? nicoPV.Name
					: string.Format("{0} {1}", ArtistString, Name));

				WebLinks.Add(new WebLinkContract(string.Format("http://www.youtube.com/results?search_query={0}", query), 
					ViewRes.Song.DetailsStrings.SearchYoutube, WebLinkCategory.Other));

			}

		}

		public string AdditionalNames { get; set; }

		public AlbumContract[] Albums { get; set; }

		[Display(Name = "Alternate versions")]
		public SongContract[] AlternateVersions { get; set; }

		public ArtistForSongContract[] Animators { get; set; }

		public string ArtistString { get; set; }

		public ArtistForSongContract[] Bands { get; set; }

		public bool CanEdit { get; set; }

		public int CommentCount { get; set; }

		public SongDetailsContract Contract { get; set; }

		public DateTime CreateDate { get; set; }

		public ContentLanguageSelection DefaultLanguageSelection { get; set; }

		public bool Deleted { get; set; }

		public bool Draft { get; set; }

		public int FavoritedTimes { get; set; }

		public int Hits { get; set; }

		public int Id { get; set; }

		public bool IsFavorited { get; set; }

		public int Length { get; set; }

		public int ListCount { get; set; }

		public string Json { get; set; }

		public CommentForApiContract[] LatestComments { get; set; }

		public int LikedTimes { get; set; }

		public LyricsForSongContract[] Lyrics { get; set; }

		public SongContract MergedTo { get; set; }

		public string Name { get; set; }

		public string NicoId { get; set; }

		public EnglishTranslatedString Notes { get; set; }

		public ArtistForSongContract[] OtherArtists { get; set; }

		public PVContract[] OriginalPVs { get; set; }

		[Display(Name = "Original version")]
		public SongForApiContract OriginalVersion { get; set; }

		public PVContract[] OtherPVs { get; set; }

		public ArtistForSongContract[] Performers { get; set; }

		public SongListBaseContract[] Pools { get; set; }

		public PVContract PrimaryPV { get; set; }

		public ArtistForSongContract[] Producers { get; set; }

		public DateTime? PublishDate { get; set; }

		public int RatingScore { get; set; }

		public ReleaseEventForApiContract ReleaseEvent { get; set; }

		public SongType SongType { get; set; }

		public EntryStatus Status { get; set; }

		public SongForApiContract[] Suggestions { get; set; }

		public TagUsageForApiContract[] Tags { get; set; }

		public string ThumbUrl { get; set; }

		public string ThumbUrlMaxSize { get; set; }

		public SongVoteRating UserRating { get; set; }

		public IList<WebLinkContract> WebLinks { get; set; }

	}

	public class SongDetailsAjax {

		public SongDetailsAjax(SongDetails model, LyricsForSongContract preferredLyrics) {

			Id = model.Id;
			UserRating = model.UserRating;
			LatestComments = model.LatestComments;
			OriginalVersion = model.OriginalVersion;

			SelectedLyricsId = preferredLyrics != null ? preferredLyrics.Id : 0;
			SelectedPvId = model.PrimaryPV != null ? model.PrimaryPV.Id : 0;
			SongType = model.SongType;
			TagUsages = model.Tags;

			var domains = new[] { "http://vocadb.net/", "http://utaitedb.net/" };
			LinkedPages = model.WebLinks.Where(w => domains.Any(d => w.Url.StartsWith(d, StringComparison.InvariantCultureIgnoreCase))).Select(w => w.Url).ToArray();

		}

		public int Id { get; set; }

		public CommentForApiContract[] LatestComments { get; set; }

		public string[] LinkedPages { get; set; }

		public SongForApiContract OriginalVersion { get; set; }

		public int SelectedLyricsId { get; set; }

		public int SelectedPvId { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public SongType SongType { get; set; }

		public TagUsageForApiContract[] TagUsages { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		public SongVoteRating UserRating { get; set; }

	}

}