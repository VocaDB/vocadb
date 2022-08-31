import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
import { VdbPlayerStore } from '@/Stores/VdbPlayer/VdbPlayerStore';
import { PVPlayer } from '@vocadb/nostalgic-diva';
import React from 'react';

interface VdbPlayerContextProps {
	vdbPlayer: VdbPlayerStore;
	playQueue: PlayQueueStore;
	playerRef: React.MutableRefObject<PVPlayer | undefined>;
}

const VdbPlayerContext = React.createContext<VdbPlayerContextProps>(undefined!);

interface VdbPlayerProviderProps {
	children?: React.ReactNode;
}

export const VdbPlayerProvider = ({
	children,
}: VdbPlayerProviderProps): React.ReactElement => {
	const [vdbPlayer] = React.useState(() => new VdbPlayerStore());

	const playerRef = React.useRef<PVPlayer>();

	return (
		<VdbPlayerContext.Provider
			value={{ vdbPlayer, playQueue: vdbPlayer.playQueue, playerRef }}
		>
			{children}
		</VdbPlayerContext.Provider>
	);
};

export const useVdbPlayer = (): VdbPlayerContextProps => {
	const vdbPlayerContext = React.useContext(VdbPlayerContext);

	return vdbPlayerContext;
};
