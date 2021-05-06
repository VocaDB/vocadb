using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service.BrandableStrings;
using VocaDb.Model.Service.BrandableStrings.Collections;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Utils.Config;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
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

		public string Name { get; init; }

		public LoginManagerProps(LoginManager model)
		{
			IsLoggedIn = model.IsLoggedIn;
			LanguagePreference = model.LanguagePreference;
			Name = model.Name;
		}
	}

	public sealed record ServerOnlyUserWithPermissionsContractProps
	{
		public int Id { get; init; }

		public string Name { get; init; }

		public int UnreadMessagesCount { get; init; }

		public ServerOnlyUserWithPermissionsContractProps(ServerOnlyUserWithPermissionsContract model)
		{
			Id = model.Id;
			Name = model.Name;
			UnreadMessagesCount = model.UnreadMessagesCount;
		}
	}

	public sealed record LoginProps
	{
		public bool CanAccessManageMenu { get; init; }

		public bool CanManageDb { get; init; }

		public bool CanManageEntryReports { get; init; }

		public LoginManagerProps Manager { get; init; }

		public ServerOnlyUserWithPermissionsContractProps? User { get; init; }

		public LoginProps(Login model)
		{
			CanAccessManageMenu = model.CanAccessManageMenu;
			CanManageDb = model.CanManageDb;
			CanManageEntryReports = model.CanManageEntryReports;
			Manager = new LoginManagerProps(model.Manager);
			User = model.User != null ? new ServerOnlyUserWithPermissionsContractProps(model.User) : null;
		}
	}

	public sealed record VocaDbPageProps
	{
		public BrandableStringsManagerProps BrandableStrings { get; init; }

		public VdbConfigManagerProps Config { get; init; }

		public MenuPageLinkProps[] AppLinks { get; init; }

		public MenuPageLinkProps[] BigBanners { get; init; }

		public MenuPageLinkProps[] SmallBanners { get; init; }

		public MenuPageLinkProps[] SocialLinks { get; init; }

		public LoginProps Login { get; init; }

		public VocaDbPageProps(VocaDbPage model, Login login)
		{
			BrandableStrings = new BrandableStringsManagerProps(model.BrandableStrings);
			Config = new VdbConfigManagerProps(model.Config);
			AppLinks = MenuPage.AppLinks.Select(l => new MenuPageLinkProps(l)).ToArray();
			BigBanners = MenuPage.BigBanners.Select(l => new MenuPageLinkProps(l)).ToArray();
			SmallBanners = MenuPage.SmallBanners.Select(l => new MenuPageLinkProps(l)).ToArray();
			SocialLinks = MenuPage.SocialLinks.Select(l => new MenuPageLinkProps(l)).ToArray();
			Login = new LoginProps(login);
		}
	}

	public class ReactController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
