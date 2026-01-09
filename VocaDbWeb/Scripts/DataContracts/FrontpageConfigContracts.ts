export interface FrontpageBannerContract {
	id: number;
	title: string;
	description: string;
	imageUrl: string;
	linkUrl: string;
	enabled: boolean;
	sortIndex: number;
}

export interface FrontpageConfigContract {
	banners: FrontpageBannerContract[];
}
