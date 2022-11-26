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

	React.useEffect(() => {
		Promise.all([
			httpClient.get<GlobalResources>(
				urlMapper.mapRelative('/api/globals/resources'),
			),
			httpClient.get<GlobalValues>(
				urlMapper.mapRelative('/api/globals/values'),
			),
		]).then(([resources, values]) => {
			i18n.changeLanguage(values.uiCulture);
			moment.locale(values.culture);

			setVdb({ resources, values });
		});
	}, [i18n]);

	return vdb ? (
		<VdbContext.Provider value={vdb}>{children}</VdbContext.Provider>
	) : (
		<></>
	);
};

export const useVdb = (): VdbContextProps => {
	return React.useContext(VdbContext);
};
