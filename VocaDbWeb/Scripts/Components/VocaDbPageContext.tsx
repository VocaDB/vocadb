import React from 'react';

interface AppConfigProps {
  staticContentHost: string;
}

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

interface ServerOnlyUserWithPermissionsContractProps {
  id: number;
  name: string;
  unreadMessagesCount: number;
}

interface LoginProps {
  canAccessManageMenu: boolean;
  canManageDb: boolean;
  canManageEntryReports: boolean;
  manager: LoginManagerProps;
  user?: ServerOnlyUserWithPermissionsContractProps;
}

export interface VocaDbPageProps {
  appConfig: AppConfigProps;
  brandableStrings: BrandableStringsManagerProps;
  config: VdbConfigManagerProps;
  rootPath: string;
  culture: string;
  uiCulture: string;
  appLinks: MenuPageLinkProps[];
  bannerUrl: string;
  bigBanners: MenuPageLinkProps[];
  blogUrl: string;
  smallBanners: MenuPageLinkProps[];
  socialLinks: MenuPageLinkProps[];
  login: LoginProps;
}

export interface VocaDbPage {
  props: VocaDbPageProps;
}

const VocaDbPageContext = React.createContext<VocaDbPage>(undefined!);

export default VocaDbPageContext;
