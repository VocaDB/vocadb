import { GlobalResources } from '@/Shared/GlobalResources';
import { GlobalValues } from '@/Shared/GlobalValues';
import React from 'react';

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
	return (
		<VdbContext.Provider value={(window as any).vdb /* TODO */}>
			{children}
		</VdbContext.Provider>
	);
};

export const useVdb = (): VdbContextProps => {
	return React.useContext(VdbContext);
};
