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
						de: 'v1.22',
						es: 'v1.18',
						fi: 'v1.19',
						ja: 'v1.21',
						en: 'v1.26',
						ru: 'v1.19',
						ko: 'v1.16',
						nl: 'v1.5',
						quc: 'v1.14',
						'zh-Hans': 'v1.3',
					},
				},
			],
		},
	});
