import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import localeLoaders from 'virtual:locales';

const lastLanguageKey = 'i18n.lastLanguage';

i18n.use(initReactI18next).init({
	load: 'all',
	fallbackLng: 'en',
	// Each language is one bundle with all namespaces (see vite.locales.ts), added by
	// changeLanguage before anything renders
	resources: {},
	interpolation: {
		escapeValue: false,
	},
	react: {
		useSuspense: false,
	},
	supportedLngs: [
		'de',
		'es',
		'fi',
		'ja',
		'en',
		'ru',
		'ko',
		'nl',
		'quc',
		'zh-Hans',
	],
});

const bundles = new Map<string, Promise<void>>();

// The language whose bundle has the strings for lng: 'ja-JP' → 'ja', unsupported → 'en'
const getBundleLanguage = (lng: string): string =>
	i18n.services.languageUtils.toResolveHierarchy(lng)[0] ?? 'en';

const loadBundle = (lng: string): Promise<void> => {
	let bundle = bundles.get(lng);
	if (!bundle) {
		const loader = localeLoaders[lng];
		bundle = loader
			? loader().then((resources) => {
					for (const [ns, resource] of Object.entries(resources)) {
						i18n.addResourceBundle(lng, ns, resource, true, true);
					}
			  })
			: Promise.resolve();
		bundle.catch(() => bundles.delete(lng));
		bundles.set(lng, bundle);
	}
	return bundle;
};

export const changeLanguage = async (lng: string): Promise<void> => {
	try {
		localStorage.setItem(lastLanguageKey, lng);
	} catch {}

	try {
		await loadBundle(getBundleLanguage(lng));
	} catch (error) {
		console.error('Failed to load translations', error);
	}

	await i18n.changeLanguage(lng);
};

const getLastLanguage = (): string | null => {
	try {
		return localStorage.getItem(lastLanguageKey);
	} catch {
		return null;
	}
};

// For anonymous users, the server picks the first of these in Accept-Language
// (InterfaceLanguage.LanguageCodes)
const serverLanguages = ['en', 'de', 'es', 'fi', 'pt', 'ru', 'ja', 'zh'];

const guessLanguage = (): string =>
	getLastLanguage() ??
	navigator.languages
		.map((lng) => lng.split('-')[0])
		.find((lng) => serverLanguages.includes(lng)) ??
	'en';

// Start downloading the translations while /api/globals/values is still loading
loadBundle(getBundleLanguage(guessLanguage())).catch(() => {});

// Translations used to be cached in localStorage by i18next-localstorage-backend
try {
	for (const key of Object.keys(localStorage)) {
		if (key.startsWith('i18next_res_')) localStorage.removeItem(key);
	}
} catch {}
