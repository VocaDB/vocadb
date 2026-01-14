import { GlobalValues } from '@/Shared/GlobalValues';

export interface BackgroundCredit {
	artistName: string;
	artistUrl: string;
}

const themeCredits: Record<string, BackgroundCredit> = {
	darkangel: {
		artistName: 'るみあ',
		artistUrl: 'https://www.pixiv.net/en/users/1291606',
	},
	tetodb: {
		artistName: 'はひふへ',
		artistUrl: 'https://www.pixiv.net/en/users/46764',
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
		artistName: 'Namie',
		artistUrl: 'https://www.pixiv.net/en/users/3829860',
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

