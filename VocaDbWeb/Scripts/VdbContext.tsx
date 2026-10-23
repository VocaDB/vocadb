import { GlobalValues } from '@/Shared/GlobalValues';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import { changeLanguage } from '@/i18n';
import dayjs from 'dayjs';
import React from 'react';

interface VdbContextProps {
	values: GlobalValues;
	refresh(): Promise<void>;
}

const locales: LocaleLoader = {
	'de-DE': () => import('dayjs/locale/de'),
	en: () => import('dayjs/locale/en'),
	'en-US': () => import('dayjs/locale/en'),
	es: () => import('dayjs/locale/es'),
	ko: () => import('dayjs/locale/ko'),
	pt: () => import('dayjs/locale/pt'),
	'fi-FI': () => import('dayjs/locale/fi'),
	'ru-RU': () => import('dayjs/locale/ru'),
	'zh-Hans': () => import('dayjs/locale/zh'),
	'ja-JP': () => import('dayjs/locale/ja'),
};

interface LocaleLoader {
	[key: string]: () => Promise<any>;
}

const VdbContext = React.createContext<VdbContextProps>(undefined!);

interface VdbProviderProps {
	children?: React.ReactNode;
}

export const VdbProvider = ({
	children,
}: VdbProviderProps): React.ReactElement => {
	const [vdb, setVdb] = React.useState<VdbContextProps>();

	const refresh = React.useCallback(async () => {
		const values = await httpClient.get<GlobalValues>(
			urlMapper.mapRelative('/api/globals/values'),
		);

		if (values.culture in locales) {
			locales[values.culture]().then(() => dayjs.locale(values.culture));
		}

		// Before the first render, so that nothing renders without its translations.
		await changeLanguage(values.uiCulture);

		setVdb({ values, refresh });
	}, []);

	React.useEffect(() => {
		refresh();
	}, [refresh]);

	return vdb ? (
		<VdbContext.Provider value={vdb}>{children}</VdbContext.Provider>
	) : (
		<></>
	);
};

export const useVdb = (): VdbContextProps => {
	return React.useContext(VdbContext);
};
