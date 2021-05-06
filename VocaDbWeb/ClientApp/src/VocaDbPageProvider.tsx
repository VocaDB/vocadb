import React from 'react';

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

export interface VocaDbPage {
  props: VocaDbPageProps;
}

const VocaDbPageContext = React.createContext<VocaDbPage>(undefined!);

export const useVocaDbPage = (): VocaDbPage =>
  React.useContext(VocaDbPageContext);

interface VocaDbPageProviderProps {
  children?: React.ReactNode;
  value: VocaDbPage;
}

const VocaDbPageProvider = ({
  children,
  value,
}: VocaDbPageProviderProps): React.ReactElement => {
  return (
    <VocaDbPageContext.Provider value={value}>
      {children}
    </VocaDbPageContext.Provider>
  );
};

export default VocaDbPageProvider;
