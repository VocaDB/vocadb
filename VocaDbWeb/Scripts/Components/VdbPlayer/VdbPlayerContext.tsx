import { albumRepo } from '@/Repositories/AlbumRepository';
import { artistRepo } from '@/Repositories/ArtistRepository';
import { eventRepo } from '@/Repositories/ReleaseEventRepository';
import { songListRepo } from '@/Repositories/SongListRepository';
import { songRepo } from '@/Repositories/SongRepository';
import { tagRepo } from '@/Repositories/TagRepository';
import { userRepo } from '@/Repositories/UserRepository';
import { PlayQueueRepositoryFactory } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
import { VdbPlayerStore } from '@/Stores/VdbPlayer/VdbPlayerStore';
import { useVdb } from '@/VdbContext';
import React from 'react';

const playQueueRepoFactory = new PlayQueueRepositoryFactory(
	songListRepo,
	songRepo,
	userRepo,
);

const VdbPlayerContext = React.createContext<VdbPlayerStore>(undefined!);

interface VdbPlayerProviderProps {
	children?: React.ReactNode;
}

export const VdbPlayerProvider = ({
	children,
}: VdbPlayerProviderProps): React.ReactElement => {
	const vdb = useVdb();

	const [vdbPlayerStore] = React.useState(
		() =>
			new VdbPlayerStore(
				vdb.values,
				albumRepo,
				eventRepo,
				songRepo,
				playQueueRepoFactory,
				artistRepo,
				tagRepo,
			),
	);

	return (
		<VdbPlayerContext.Provider value={vdbPlayerStore}>
			{children}
		</VdbPlayerContext.Provider>
	);
};

export const useVdbPlayer = (): VdbPlayerStore => {
	return React.useContext(VdbPlayerContext);
};

export const usePlayQueue = (): PlayQueueStore => {
	const vdbPlayer = useVdbPlayer();
	return vdbPlayer.playQueue;
};
