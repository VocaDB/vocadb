'use client';

import { apiGet } from '@/Helpers/FetchApiHelper';
import { GlobalValues } from '@/types/GlobalValues';
import React, { useEffect } from 'react';

interface VdbContext {
	values: GlobalValues;
}

const VdbContext = React.createContext<VdbContext>(undefined!);

interface VdbProviderProps {
	children?: React.ReactNode;
	initialValue?: GlobalValues;
}

export const VdbProvider = ({ children, initialValue }: VdbProviderProps): React.ReactElement => {
	const [vdb, setVdb] = React.useState<GlobalValues | undefined>(initialValue);

	useEffect(() => {
		if (vdb === undefined) {
			// TODO: Error handling
			apiGet<GlobalValues>('/api/globals/values', { credentials: true })
				.then((val) => setVdb(val))
				.catch(() => console.log('Invalid vdb-values cookie'));
		}
	}, [vdb]);

	return vdb ? (
		<VdbContext.Provider value={{ values: vdb }}>{children}</VdbContext.Provider>
	) : (
		<></>
	);
};

export const useVdb = (): VdbContext => {
	return React.useContext(VdbContext);
};

