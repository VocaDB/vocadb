import { GlobalValues } from '@/Shared/GlobalValues';

export interface BackgroundCredit {
	artistName: string;
	artistUrl: string;
}

const themeCredits: Record<string, BackgroundCredit> = {
	darkangel: {
		artistName: 'みゆ',
		artistUrl: 'https://piapro.jp/t/nire',
	},
	tetodb: {
		artistName: 'みゆ',
		artistUrl: 'https://piapro.jp/t/nire',
	},
	pride: {
		artistName: 'chiyo',
		artistUrl: 'https://twitter.com/seeuchiyo',
	},
	utaite: {
		artistName: 'sorappane',
		artistUrl: 'https://sorappane.carrd.co/',
	},
	touhou: {
		artistName: 'みゆ',
		artistUrl: 'https://piapro.jp/t/nire',
	},
};

const defaultCredit: BackgroundCredit = {
	artistName: 'みゆ',
	artistUrl: 'https://piapro.jp/t/nire',
};

export function getBackgroundCredit(values: GlobalValues): BackgroundCredit {
	const stylesheet = values.loggedUser?.stylesheet?.toLowerCase();
	if (stylesheet) {
		for (const [themeKey, credit] of Object.entries(themeCredits)) {
			if (stylesheet.startsWith(themeKey)) {
				return credit;
			}
		}
	}

	const siteName = values.siteName.toLowerCase();
	for (const [themeKey, credit] of Object.entries(themeCredits)) {
		if (siteName.includes(themeKey)) {
			return credit;
		}
	}

	return defaultCredit;
}
