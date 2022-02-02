import i18n from 'i18next';
import ChainedBackend from 'i18next-chained-backend';
import HttpBackend from 'i18next-http-backend';
import LocalStorageBackend from 'i18next-localstorage-backend';
import moment from 'moment';
import { initReactI18next } from 'react-i18next';

i18n
	.use(ChainedBackend)
	.use(initReactI18next)
	.init({
		lng: vdb.values.uiCulture,
		load: 'currentOnly',
		fallbackLng: 'en',
		interpolation: {
			escapeValue: false,
		},
		react: {
			useSuspense: false,
		},
		backend: {
			backends: [LocalStorageBackend, HttpBackend],
			backendOptions: [
				{
					expirationTime: 7 * 24 * 60 * 60 * 1000, // 7 days
				},
				{},
			],
		},
	});

moment.locale(vdb.values.culture);
