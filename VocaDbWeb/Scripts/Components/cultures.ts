export const cultures = Object.fromEntries(
	Object.entries({
		'en-US': {
			nativeName: 'English (United States)',
			englishName: 'English (United States)',
		},
		'de-DE': {
			nativeName: 'Deutsch (Deutschland)',
			englishName: 'German (Germany)',
		},
		es: { nativeName: 'espa\u00F1ol', englishName: 'Spanish' },
		'fi-Fi': { nativeName: 'suomi (Suomi)', englishName: 'Finnish (Finland)' },
		pt: { nativeName: 'portugu\u00EAs', englishName: 'Portuguese' },
		'ru-RU': {
			nativeName:
				'\u0440\u0443\u0441\u0441\u043A\u0438\u0439 (\u0420\u043E\u0441\u0441\u0438\u044F)',
			englishName: 'Russian (Russia)',
		},
		'ja-JP': {
			nativeName: '\u65E5\u672C\u8A9E (\u65E5\u672C)',
			englishName: 'Japanese (Japan)',
		},
		'zh-Hans': {
			nativeName: '\u4E2D\u6587\uFF08\u7B80\u4F53\uFF09',
			englishName: 'Chinese (Simplified)',
		},
	}).orderBy(([_, value]) => value.nativeName),
); /* TODO */
