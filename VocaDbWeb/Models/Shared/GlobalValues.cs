using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Shared;

public sealed record GlobalValues
{
	public sealed record MenuPageLink
	{
		public string BannerImg { get; init; }
		public string Title { get; init; }
		public string Url { get; init; }

		public MenuPageLink(MenuPage.Link model)
		{
			BannerImg = model.BannerImg;
			Title = model.Title;
			Url = model.Url;
		}
	}

	public bool AllowCustomArtistName { get; init; }
	[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	public DiscType[] AlbumTypes { get; init; }
	public bool AllowCustomTracks { get; init; }
	[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	public ArtistType[] ArtistTypes { get; init; }
	[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	public ArtistRoles[] ArtistRoles { get; init; }
	public string? ExternalHelpPath { get; init; }
	public string? HostAddress { get; init; }
	public string? LockdownMessage { get; init; }
	[JsonProperty(ItemConverterType = typeof(StringEnumConverter))]
	public SongType[] SongTypes { get; init; }
	public EventCategory[] EventTypes { get; init; }
	public string StaticContentHost { get; init; }

	public string SiteName { get; init; }
	public string SiteTitle { get; init; }

	public string? BannerUrl { get; init; }
	public string? BlogUrl { get; init; }
	public string? PatreonLink { get; init; }
	public string? SitewideAnnouncement { get; init; }
	public string[] Stylesheets { get; init; }
	public FrontpageBannerContract[] FrontpageBanners { get; init; }

	public string AmazonComAffiliateId { get; init; }
	public string AmazonJpAffiliateId { get; init; }
	public string PlayAsiaAffiliateId { get; init; }
	public int FreeTagId { get; init; }
	public int InstrumentalTagId { get; init; }

	[JsonConverter(typeof(StringEnumConverter))]
	public ContentLanguagePreference LanguagePreference { get; init; }
	public bool IsLoggedIn { get; init; }
	public int LoggedUserId { get; init; }
	public SanitizedUserWithPermissionsContract? LoggedUser { get; init; }
	public string Culture { get; init; }
	public string UICulture { get; init; }

	public string Slogan { get; init; }

	public MenuPageLink[] AppLinks { get; init; }
	public MenuPageLink[] BigBanners { get; init; }
	public MenuPageLink[] SmallBanners { get; init; }
	public MenuPageLink[] SocialLinks { get; init; }

	public bool SignupsDisabled { get; init; }
	public string ReCAPTCHAPublicKey { get; init; }
	public bool AlwaysPermissions { get; init; }

	public GlobalValues(
		BrandableStringsManager brandableStrings,
		VdbConfigManager config,
		IUserPermissionContext userContext,
		FrontpageConfigService frontpageConfigService
	)
	{
		AllowCustomArtistName = AppConfig.AllowCustomArtistName;
		AlbumTypes = AppConfig.AlbumTypes;
		AllowCustomTracks = AppConfig.AllowCustomTracks;
		ArtistTypes = AppConfig.ArtistTypes;
		ArtistRoles = AppConfig.ArtistRoles;
		ExternalHelpPath = AppConfig.ExternalHelpPath;
		HostAddress = AppConfig.HostAddress;
		LockdownMessage = AppConfig.LockdownMessage;
		EventTypes = AppConfig.EventTypes;
		SongTypes = AppConfig.SongTypes;
		StaticContentHost = AppConfig.StaticContentHost;

		SiteName = brandableStrings.SiteName;
		SiteTitle = brandableStrings.SiteTitle;

		BannerUrl = config.SiteSettings.BannerUrl.EmptyToNull();
		BlogUrl = config.SiteSettings.BlogUrl.EmptyToNull();
		PatreonLink = config.SiteSettings.PatreonLink.EmptyToNull();
		SitewideAnnouncement = config.SiteSettings.SitewideAnnouncement.EmptyToNull();
		Stylesheets = AppConfig.SiteSettings.Stylesheets?.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

		AmazonComAffiliateId = config.Affiliates.AmazonComAffiliateId;
		AmazonJpAffiliateId = config.Affiliates.amazonJpAffiliateId;
		PlayAsiaAffiliateId = config.Affiliates.PlayAsiaAffiliateId;
		FreeTagId = config.SpecialTags.Free;
		InstrumentalTagId = config.SpecialTags.Instrumental;

		LanguagePreference = userContext.LanguagePreference;
		IsLoggedIn = userContext.IsLoggedIn;
		LoggedUserId = userContext.LoggedUserId;
		LoggedUser = userContext.LoggedUser is ServerOnlyUserWithPermissionsContract loggedUser ? new SanitizedUserWithPermissionsContract(loggedUser) : null;
		Culture = Thread.CurrentThread.CurrentCulture.Name;
		UICulture = Thread.CurrentThread.CurrentUICulture.Name;

		Slogan = SloganGenerator.Generate();

		// Load frontpage config from database
		var frontpageConfig = frontpageConfigService.GetConfig();
		FrontpageBanners = frontpageConfig.Banners.Where(b => b.Enabled).ToArray();

		AppLinks = MenuPage.AppLinks.Select(l => new MenuPageLink(l)).ToArray();
		BigBanners = MenuPage.BigBanners.Select(l => new MenuPageLink(l)).ToArray();
		SmallBanners = MenuPage.SmallBanners.Select(l => new MenuPageLink(l)).ToArray();
		SocialLinks = MenuPage.SocialLinks.Select(l => new MenuPageLink(l)).ToArray();

		SignupsDisabled = AppConfig.SiteSettings.SignupsDisabled;
		ReCAPTCHAPublicKey = AppConfig.ReCAPTCHAPublicKey;
		AlwaysPermissions = AppConfig.AlwaysPermissions;
	}
}
