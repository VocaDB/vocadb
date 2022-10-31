import { MutedUsersStore } from '@/Stores/MutedUsersStore';
import { useLocalStorageStateStore } from '@vocadb/route-sphere';
import React from 'react';

interface AppContextProps {
	mutedUsers: MutedUsersStore;
}

const AppContext = React.createContext<AppContextProps>(undefined!);

const mutedUsersStore = new MutedUsersStore();

interface AppProviderProps {
	children?: React.ReactNode;
}

export const AppProvider = ({
	children,
}: AppProviderProps): React.ReactElement => {
	useLocalStorageStateStore('MutedUsersStore', mutedUsersStore);

	return (
		<AppContext.Provider value={{ mutedUsers: mutedUsersStore }}>
			{children}
		</AppContext.Provider>
	);
};

const useAppContext = (): AppContextProps => {
	return React.useContext(AppContext);
};

export const useMutedUsers = (): MutedUsersStore => {
	const { mutedUsers } = useAppContext();
	return mutedUsers;
};
