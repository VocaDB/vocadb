import { iso6393 } from 'iso-639-3';

export const extendedUserLanguageCultures = Object.fromEntries(
	iso6393.map((lang) => [
		lang.iso6391 ?? lang.iso6392B ?? lang.iso6392T ?? lang.iso6393,
		{
			englishName: lang.name,
			nativeName: lang.name,
		},
	]),
);
