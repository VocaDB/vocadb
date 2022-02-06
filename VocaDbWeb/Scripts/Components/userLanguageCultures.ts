import _ from 'lodash';

export const userLanguageCultures: Record<
	string,
	{ nativeName: string; englishName: string }
> = _.chain({
	de: { nativeName: 'Deutsch', englishName: 'German' },
	en: { nativeName: 'English', englishName: 'English' },
	es: { nativeName: 'espa\u00F1ol', englishName: 'Spanish' },
	fil: { nativeName: 'Filipino', englishName: 'Filipino' },
	fr: { nativeName: 'fran\u00E7ais', englishName: 'French' },
	id: { nativeName: 'Indonesia', englishName: 'Indonesian' },
	it: { nativeName: 'italiano', englishName: 'Italian' },
	nl: { nativeName: 'Nederlands', englishName: 'Dutch' },
	no: { nativeName: 'norsk', englishName: 'Norwegian' },
	pl: { nativeName: 'polski', englishName: 'Polish' },
	pt: { nativeName: 'portugu\u00EAs', englishName: 'Portuguese' },
	fi: { nativeName: 'suomi', englishName: 'Finnish' },
	sv: { nativeName: 'svenska', englishName: 'Swedish' },
	ru: {
		nativeName: '\u0440\u0443\u0441\u0441\u043A\u0438\u0439',
		englishName: 'Russian',
	},
	th: { nativeName: '\u0E44\u0E17\u0E22', englishName: 'Thai' },
	ko: { nativeName: '\uD55C\uAD6D\uC5B4', englishName: 'Korean' },
	zh: { nativeName: '\u4E2D\u6587', englishName: 'Chinese' },
	ja: { nativeName: '\u65E5\u672C\u8A9E', englishName: 'Japanese' },
})
	.toPairs()
	.orderBy(([_, value]) => value.nativeName)
	.fromPairs()
	.value(); /* TODO */
