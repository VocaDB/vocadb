using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Shared
{
	public sealed record GlobalResources
	{
		public sealed record AlbumResources
		{
			public string? NewAlbumArtistDesc { get; }
			public string? NewAlbumInfo { get; }

			public AlbumResources(AlbumStrings model)
			{
				NewAlbumArtistDesc = model.NewAlbumArtistDesc;
				NewAlbumInfo = model.NewAlbumInfo;
			}
		}

		public sealed record ArtistResources
		{
			public string? AuthoredBy { get; }
			public string? NewArtistExternalLink { get; }

			public ArtistResources(ArtistStrings model)
			{
				AuthoredBy = model.AuthoredBy;
				NewArtistExternalLink = model.NewArtistExternalLink;
			}
		}

		public sealed record HomeResources
		{
			public string? Welcome { get; }
			public string? WelcomeSubtitle { get; }

			public HomeResources(HomeStrings model)
			{
				Welcome = model.Welcome;
				WelcomeSubtitle = model.WelcomeSubtitle;
			}
		}

		public sealed record LayoutResources
		{
			public string? PaypalDonateTitle { get; }

			public LayoutResources(LayoutStrings model)
			{
				PaypalDonateTitle = model.PaypalDonateTitle;
			}
		}

		public sealed record SongResources
		{
			public string? NewSongInfo { get; }
			public string? RankingsTitle { get; }

			public SongResources(SongStrings model)
			{
				NewSongInfo = model.NewSongInfo;
				RankingsTitle = model.RankingsTitle;
			}
		}

		public sealed record UserResources
		{
			public string? RequestVerificationInfo { get; }

			public UserResources(UserStrings model)
			{
				RequestVerificationInfo = model.RequestVerificationInfo;
			}
		}

		public AlbumResources Album { get; }
		public ArtistResources Artist { get; }
		public HomeResources Home { get; }
		public LayoutResources Layout { get; }
		public SongResources Song { get; }
		public UserResources User { get; }

		public GlobalResources(VocaDbPage model)
		{
			Album = new AlbumResources(model.BrandableStrings.Album);
			Artist = new ArtistResources(model.BrandableStrings.Artist);
			Home = new HomeResources(model.BrandableStrings.Home);
			Layout = new LayoutResources(model.BrandableStrings.Layout);
			Song = new SongResources(model.BrandableStrings.Song);
			User = new UserResources(model.BrandableStrings.User);
		}
	}
}
