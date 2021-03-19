using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Config;
using VocaDb.ReMikus;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Middleware
{
	public sealed record LayoutStringsProps
	{
		public string PaypalDonateTitle { get; init; }

		public string SiteName { get; init; }

		public LayoutStringsProps(LayoutStrings model)
		{
			PaypalDonateTitle = model.PaypalDonateTitle;
			SiteName = model.SiteName;
		}
	}

	public sealed record BrandableStringsManagerProps
	{
		public LayoutStringsProps Layout { get; init; }

		public string SiteName { get; init; }

		public BrandableStringsManagerProps(BrandableStringsManager model)
		{
			Layout = new LayoutStringsProps(model.Layout);
			SiteName = model.SiteName;
		}
	}

	public sealed record SiteSettingsSectionProps
	{
		public string PatreonLink { get; init; }

		public SiteSettingsSectionProps(SiteSettingsSection model)
		{
			PatreonLink = model.PatreonLink;
		}
	}

	public sealed record VdbConfigManagerProps
	{
		public SiteSettingsSectionProps SiteSettings { get; init; }

		public VdbConfigManagerProps(VdbConfigManager model)
		{
			SiteSettings = new SiteSettingsSectionProps(model.SiteSettings);
		}
	}

	public sealed record MenuPageLinkProps
	{
		public string BannerImg { get; init; }

		public string Title { get; init; }

		public string Url { get; init; }

		public MenuPageLinkProps(MenuPage.Link model)
		{
			BannerImg = model.BannerImg;
			Title = model.Title;
			Url = model.Url;
		}
	}

	public sealed record LoginManagerProps
	{
		public bool IsLoggedIn { get; init; }

		public ContentLanguagePreference LanguagePreference { get; init; }

		public string Name { get; set; }

		public LoginManagerProps(LoginManager value)
		{
			IsLoggedIn = value.IsLoggedIn;
			LanguagePreference = value.LanguagePreference;
			Name = value.Name;
		}
	}

	public sealed record LoginProps
	{
		public bool CanAccessManageMenu { get; init; }

		public bool CanManageDb { get; init; }

		public bool CanManageEntryReports { get; init; }

		public LoginManagerProps Manager { get; init; }

		public UserBaseContract? User { get; init; }

		public LoginProps(Login model)
		{
			CanAccessManageMenu = model.CanAccessManageMenu;
			CanManageDb = model.CanManageDb;
			CanManageEntryReports = model.CanManageEntryReports;
			Manager = new LoginManagerProps(model.Manager);
			User = model.User != null ? new UserBaseContract(model.User) : null;
		}
	}

	public sealed record VocaDbPageProps
	{
		public BrandableStringsManagerProps BrandableStrings { get; init; } = default!;

		public VdbConfigManagerProps Config { get; init; } = default!;

		public MenuPageLinkProps[] AppLinks { get; init; } = default!;

		public MenuPageLinkProps[] BigBanners { get; init; } = default!;

		public MenuPageLinkProps[] SmallBanners { get; init; } = default!;

		public MenuPageLinkProps[] SocialLinks { get; init; } = default!;

		public LoginProps Login { get; init; } = default!;
	}

	public class HandleInertiaRequests
	{
		private readonly RequestDelegate _next;

		public HandleInertiaRequests(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context, LaravelMix laravelMix)
		{
			var config = AppConfig.GetGlobalLinksSection();

			// shared data
			Inertia.SharedProps = new VocaDbPageProps
			{
				BrandableStrings = new BrandableStringsManagerProps(context.RequestServices.GetRequiredService<BrandableStringsManager>()),
				Config = new VdbConfigManagerProps(context.RequestServices.GetRequiredService<VdbConfigManager>()),

				AppLinks = config?.AppLinks?.Links.Select(l => new MenuPageLinkProps(new MenuPage.Link(l.Title, l.Url, l.BannerImg))).ToArray() ?? Array.Empty<MenuPageLinkProps>(),
				BigBanners = config?.BigBanners?.Links.Select(l => new MenuPageLinkProps(new MenuPage.Link(l.Title, l.Url, l.BannerImg)))/*.RandomSort()*/.ToArray() ?? Array.Empty<MenuPageLinkProps>(),
				SmallBanners = config?.SmallBanners?.Links.Select(l => new MenuPageLinkProps(new MenuPage.Link(l.Title, l.Url, l.BannerImg)))/*.RandomSort()*/.ToArray() ?? Array.Empty<MenuPageLinkProps>(),
				SocialLinks = config?.SocialSites?.Links.Select(l => new MenuPageLinkProps(new MenuPage.Link(l.Title, l.Url, l.BannerImg))).ToArray() ?? Array.Empty<MenuPageLinkProps>(),

				Login = new LoginProps(context.RequestServices.GetRequiredService<Login>()),
			};

			// asset versioning
			using var md5 = MD5.Create();
			using var stream = File.OpenRead(laravelMix.ManifestPath);
			var hash = await md5.ComputeHashAsync(stream);
			Inertia.VersionSelector = () => BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

			await _next(context);
		}
	}
}
