import i18n from 'i18next';
import I18NextChainedBackend from 'i18next-chained-backend';
import I18NextHttpBackend from 'i18next-http-backend';
import I18NextLocalStorageBackend from 'i18next-localstorage-backend';
import { initReactI18next } from 'react-i18next';

i18n
	.use(I18NextChainedBackend)
	.use(initReactI18next)
	.init({
		load: 'all',
		fallbackLng: 'en',
		interpolation: {
			escapeValue: false,
		},
		react: {
			useSuspense: false,
		},
		supportedLngs: ['de', 'es', 'fi', 'ja', 'en', 'ru', 'ko', 'quc'],
		backend: {
			backends: [I18NextLocalStorageBackend, I18NextHttpBackend],
			backendOptions: [
				{
					expirationTime: 7 * 24 * 60 * 60 * 1000, // 7 days
					versions: {
						de: 'v1.9',
						es: 'v1.7',
						fi: 'v1.8',
						ja: 'v1.8',
						en: 'v1.8',
						ru: 'v1.8',
						ko: 'v1.5',
						quc: 'v1.2',
					},
				},
			],
		},
	});
