import { VdbPlayerStore } from '@/Stores/VdbPlayer/VdbPlayerStore';
import { PVPlayer } from '@vocadb/nostalgic-diva';
import React from 'react';

const VdbPlayerContext = React.createContext<{
	vdbPlayer: VdbPlayerStore;
	playerRef: React.MutableRefObject<PVPlayer | undefined>;
}>(undefined!);

interface VdbPlayerProviderProps {
	children?: React.ReactNode;
}

export const VdbPlayerProvider = ({
	children,
}: VdbPlayerProviderProps): React.ReactElement => {
	const [vdbPlayer] = React.useState(() => new VdbPlayerStore());

	const playerRef = React.useRef<PVPlayer>();

	return (
		<VdbPlayerContext.Provider value={{ vdbPlayer, playerRef }}>
			{children}
		</VdbPlayerContext.Provider>
	);
};

export const useVdbPlayer = (): {
	vdbPlayer: VdbPlayerStore;
	playerRef: React.MutableRefObject<PVPlayer | undefined>;
} => {
	const vdbPlayerContext = React.useContext(VdbPlayerContext);

	return vdbPlayerContext;
};
