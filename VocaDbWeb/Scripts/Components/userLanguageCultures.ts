export const CULTURE_CODE_OTHER = 'qot';
export const CULTURE_CODE_UNKNOWN = 'und';

export const userLanguageCultures = Object.fromEntries(
	Object.entries({
		de: { nativeName: 'Deutsch', englishName: 'German' },
		en: { nativeName: 'English', englishName: 'English' },
		es: { nativeName: 'espa\u00F1ol', englishName: 'Spanish' },
		tl: { nativeName: 'Filipino', englishName: 'Filipino' },
		fr: { nativeName: 'fran\u00E7ais', englishName: 'French' },
		id: { nativeName: 'Indonesia', englishName: 'Indonesian' },
		it: { nativeName: 'italiano', englishName: 'Italian' },
		nl: { nativeName: 'Nederlands', englishName: 'Dutch' },
		no: { nativeName: 'norsk', englishName: 'Norwegian' },
		[CULTURE_CODE_OTHER]: { nativeName: 'Other', englishName: 'Other' },
		pl: { nativeName: 'polski', englishName: 'Polish' },
		pt: { nativeName: 'portugu\u00EAs', englishName: 'Portuguese' },
		fi: { nativeName: 'suomi', englishName: 'Finnish' },
		sv: { nativeName: 'svenska', englishName: 'Swedish' },
		ru: {
			nativeName: '\u0440\u0443\u0441\u0441\u043A\u0438\u0439',
			englishName: 'Russian',
		},
		th: { nativeName: '\u0E44\u0E17\u0E22', englishName: 'Thai' },
		[CULTURE_CODE_UNKNOWN]: { nativeName: 'Unknown', englishName: 'Unknown' },
		ko: { nativeName: '\uD55C\uAD6D\uC5B4', englishName: 'Korean' },
		zh: { nativeName: '\u4E2D\u6587', englishName: 'Chinese' },
		ja: { nativeName: '\u65E5\u672C\u8A9E', englishName: 'Japanese' },
	}).orderBy(([_, value]) => value.nativeName),
); /* TODO */
