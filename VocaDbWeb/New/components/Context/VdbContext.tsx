'use client';

import { GlobalValues } from '@/types/GlobalValues';
import React, { useEffect } from 'react';

const VdbContext = React.createContext<GlobalValues>(undefined!);

interface VdbProviderProps {
	children?: React.ReactNode;
	initialValue?: GlobalValues;
}

export const VdbProvider = ({ children, initialValue }: VdbProviderProps): React.ReactElement => {
	const [vdb, setVdb] = React.useState<GlobalValues | undefined>(initialValue);

	useEffect(() => {
		if (vdb === undefined) {
			// TODO: Error handling
			fetch('/api/globals/values')
				.then((resp) => resp.json())
				.then((val) => setVdb(val));
		}
	}, [vdb]);

	return vdb ? <VdbContext.Provider value={vdb}>{children}</VdbContext.Provider> : <></>;
};

export const useVdb = (): GlobalValues => {
	return React.useContext(VdbContext);
};

