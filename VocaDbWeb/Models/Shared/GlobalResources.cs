using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Shared
{
	public sealed record GlobalResources
	{
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
			public string? RankingsTitle { get; }

			public SongResources(SongStrings model)
			{
				RankingsTitle = model.RankingsTitle;
			}
		}

		public ArtistResources Artist { get; }
		public HomeResources Home { get; }
		public LayoutResources Layout { get; }
		public SongResources Song { get; }

		public GlobalResources(VocaDbPage model)
		{
			Artist = new ArtistResources(model.BrandableStrings.Artist);
			Home = new HomeResources(model.BrandableStrings.Home);
			Layout = new LayoutResources(model.BrandableStrings.Layout);
			Song = new SongResources(model.BrandableStrings.Song);
		}
	}
}
