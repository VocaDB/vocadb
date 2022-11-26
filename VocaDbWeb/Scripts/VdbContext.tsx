import { GlobalResources } from '@/Shared/GlobalResources';
import { GlobalValues } from '@/Shared/GlobalValues';
import { httpClient } from '@/Shared/HttpClient';
import { urlMapper } from '@/Shared/UrlMapper';
import moment from 'moment';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface VdbContextProps {
	resources: GlobalResources;
	values: GlobalValues;
	refresh(): Promise<void>;
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

		i18n.changeLanguage(values.uiCulture);
		moment.locale(values.culture);

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
