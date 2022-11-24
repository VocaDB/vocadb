import { LoginManager } from '@/Models/LoginManager';
import { useVdb } from '@/VdbContext';
import React from 'react';

const LoginManagerContext = React.createContext<LoginManager>(undefined!);

interface LoginManagerProviderProps {
	children?: React.ReactNode;
}

export const LoginManagerProvider = ({
	children,
}: LoginManagerProviderProps): React.ReactElement => {
	const vdb = useVdb();

	return (
		<LoginManagerContext.Provider value={new LoginManager(vdb.values)}>
			{children}
		</LoginManagerContext.Provider>
	);
};

export const useLoginManager = (): LoginManager => {
	return React.useContext(LoginManagerContext);
};
