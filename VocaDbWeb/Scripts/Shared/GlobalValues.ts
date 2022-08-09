import { UserWithPermissionsContract } from '@/DataContracts/User/UserWithPermissionsContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { SongType } from '@/Models/Songs/SongType';

interface MenuPageLink {
	bannerImg: string;
	title: string;
	url: string;
}

// Corresponds to the GlobalValues record class in C#.
export interface GlobalValues {
	allowCustomArtistName: boolean;
	albumTypes: AlbumType[];
	allowCustomTracks: boolean;
	artistTypes: ArtistType[];
	artistRoles: string[] /* TODO: enum */;
	externalHelpPath?: string;
	hostAddress: string;
	lockdownMessage?: string;
	songTypes: SongType[];
	staticContentHost: string;

	siteName: string;
	siteTitle: string;

	bannerUrl?: string;
	blogUrl?: string;
	patreonLink?: string;
	sitewideAnnouncement?: string;

	amazonComAffiliateId: string;
	amazonJpAffiliateId: string;
	playAsiaAffiliateId: string;
	freeTagId: number;
	instrumentalTagId: number;

	/** URL of the site path, for example "/" */
	baseAddress: string;
	languagePreference: ContentLanguagePreference;
	/** Whether the user is logged in. */
	isLoggedIn: boolean;
	loggedUserId: number;
	loggedUser?: UserWithPermissionsContract;
	culture: string;
	/** UI language code, for example "en" */
	uiCulture: string;

	slogan: string;

	appLinks: MenuPageLink[];
	bigBanners: MenuPageLink[];
	smallBanners: MenuPageLink[];
	socialLinks: MenuPageLink[];

	requestToken?: string;
}
