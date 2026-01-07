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
						de: 'v1.20',
						es: 'v1.16',
						fi: 'v1.17',
						ja: 'v1.19',
						en: 'v1.24',
						ru: 'v1.17',
						ko: 'v1.14',
						nl: 'v1.3',
						quc: 'v1.12',
						'zh-Hans': 'v1.1',
					},
				},
			],
		},
	});
