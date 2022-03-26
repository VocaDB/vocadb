import VdbPlayerStore from '@Stores/VdbPlayer/VdbPlayerStore';
import React from 'react';

const VdbPlayerContext = React.createContext<VdbPlayerStore>(undefined!);

export const VdbPlayerProvider = VdbPlayerContext.Provider;

export const useVdbPlayer = (): VdbPlayerStore => {
	const miniPlayerContext = React.useContext(VdbPlayerContext);

	return miniPlayerContext;
};
