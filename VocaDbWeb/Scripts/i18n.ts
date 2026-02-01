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
		backend: {
			backends: [I18NextLocalStorageBackend, I18NextHttpBackend],
			backendOptions: [
				{
					expirationTime: 7 * 24 * 60 * 60 * 1000, // 7 days
					versions: {
						de: 'v1.21',
						es: 'v1.17',
						fi: 'v1.18',
						ja: 'v1.20',
						en: 'v1.26',
						ru: 'v1.18',
						ko: 'v1.15',
						nl: 'v1.4',
						quc: 'v1.13',
						'zh-Hans': 'v1.2',
					},
				},
			],
		},
	});
