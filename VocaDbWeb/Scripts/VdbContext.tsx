import { GlobalResources } from '@/Shared/GlobalResources';
import { GlobalValues } from '@/Shared/GlobalValues';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import dayjs from 'dayjs';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface VdbContextProps {
	resources: GlobalResources;
	values: GlobalValues;
	refresh(): Promise<void>;
}

const locales: LocaleLoader = {
	'de-DE': () => import('dayjs/locale/de'),
	en: () => import('dayjs/locale/en'),
	'en-US': () => import('dayjs/locale/en'),
	es: () => import('dayjs/locale/es'),
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

	const { i18n } = useTranslation();

	const refresh = React.useCallback(async () => {
		const [resources, values] = await Promise.all([
			httpClient.get<GlobalResources>(
				urlMapper.mapRelative('/api/globals/resources'),
			),
			httpClient.get<GlobalValues>(
				urlMapper.mapRelative('/api/globals/values'),
			),
		]);

		if (values.culture in locales) {
			locales[values.culture]().then(() => dayjs.locale(values.culture));
		}

		i18n.changeLanguage(values.uiCulture);

		setVdb({ resources, values, refresh });
	}, [i18n]);

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
