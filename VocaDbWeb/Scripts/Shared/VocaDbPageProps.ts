import { PageProps } from "@inertiajs/inertia";
import UserBaseContract from "../../wwwroot/Scripts/DataContracts/User/UserBaseContract";

interface LayoutStringsProps {
	paypalDonateTitle: string;
	siteName: string;
}

interface BrandableStringsManagerProps {
	layout: LayoutStringsProps;
	siteName: string;
}

interface SiteSettingsSectionProps {
	patreonLink: string;
}

interface VdbConfigManagerProps {
	siteSettings: SiteSettingsSectionProps;
}

interface MenuPageLinkProps {
	bannerImg: string;
	title: string;
	url: string;
}

interface LoginManagerProps {
	isLoggedIn: boolean;
	languagePreference: string;
	name: string;
}

interface LoginProps {
	canAccessManageMenu: boolean;
	canManageDb: boolean;
	canManageEntryReports: boolean;
	manager: LoginManagerProps;
	user: UserBaseContract;
}

/* TODO */
export default interface VocaDbPageProps extends PageProps {
	brandableStrings: BrandableStringsManagerProps;
	config: VdbConfigManagerProps;

	appLinks: MenuPageLinkProps[];
	bannerUrl: string;
	bigBanners: MenuPageLinkProps[];
	blogUrl: string;
	smallBanners: MenuPageLinkProps[];
	socialLinks: MenuPageLinkProps[];

	login: LoginProps;
}
