import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { PlayQueueRepositoryFactory } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
import { VdbPlayerStore } from '@/Stores/VdbPlayer/VdbPlayerStore';
import { PlayerApi } from '@vocadb/nostalgic-diva';
import React from 'react';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const songListRepo = new SongListRepository(httpClient, urlMapper);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

const playQueueRepoFactory = new PlayQueueRepositoryFactory(
	songListRepo,
	songRepo,
	userRepo,
);

interface VdbPlayerContextProps {
	vdbPlayer: VdbPlayerStore;
	playQueue: PlayQueueStore;
	playerRef: React.MutableRefObject<PlayerApi | undefined>;
}

const VdbPlayerContext = React.createContext<VdbPlayerContextProps>(undefined!);

interface VdbPlayerProviderProps {
	children?: React.ReactNode;
}

export const VdbPlayerProvider = ({
	children,
}: VdbPlayerProviderProps): React.ReactElement => {
	const [vdbPlayer] = React.useState(
		() =>
			new VdbPlayerStore(
				vdb.values,
				albumRepo,
				eventRepo,
				songRepo,
				playQueueRepoFactory,
			),
	);

	const playerRef = React.useRef<PlayerApi>();

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
