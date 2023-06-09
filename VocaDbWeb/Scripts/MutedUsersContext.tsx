import { MutedUsersStore } from '@/Stores/MutedUsersStore';
import { useLocalStorageStateStore } from '@/route-sphere';
import React from 'react';

const MutedUsersContext = React.createContext<MutedUsersStore>(undefined!);

const mutedUsersStore = new MutedUsersStore();

interface MutedUsersProviderProps {
	children?: React.ReactNode;
}

export const MutedUsersProvider = ({
	children,
}: MutedUsersProviderProps): React.ReactElement => {
	useLocalStorageStateStore('MutedUsersStore', mutedUsersStore);

	return (
		<MutedUsersContext.Provider value={mutedUsersStore}>
			{children}
		</MutedUsersContext.Provider>
	);
};

export const useMutedUsers = (): MutedUsersStore => {
	return React.useContext(MutedUsersContext);
};
