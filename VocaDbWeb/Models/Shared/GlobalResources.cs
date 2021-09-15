using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Shared
{
	public sealed record GlobalResources
	{
		public sealed record ArtistResources
		{
			public string? AuthoredBy { get; }

			public ArtistResources(ArtistStrings model)
			{
				AuthoredBy = model.AuthoredBy;
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
		public LayoutResources Layout { get; }
		public SongResources Song { get; }

		public GlobalResources(VocaDbPage model)
		{
			Artist = new ArtistResources(model.BrandableStrings.Artist);
			Layout = new LayoutResources(model.BrandableStrings.Layout);
			Song = new SongResources(model.BrandableStrings.Song);
		}
	}
}
