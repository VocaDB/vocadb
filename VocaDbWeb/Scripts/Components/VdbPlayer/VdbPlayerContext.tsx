import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { ReleaseEventRepository } from '@/Repositories/ReleaseEventRepository';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { TagRepository } from '@/Repositories/TagRepository';
import { UserRepository } from '@/Repositories/UserRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { UrlMapper } from '@/Shared/UrlMapper';
import { PlayQueueRepositoryFactory } from '@/Stores/VdbPlayer/PlayQueueRepository';
import { PlayQueueStore } from '@/Stores/VdbPlayer/PlayQueueStore';
import { VdbPlayerStore } from '@/Stores/VdbPlayer/VdbPlayerStore';
import React from 'react';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const albumRepo = new AlbumRepository(httpClient, vdb.values.baseAddress);
const artistRepo = new ArtistRepository(httpClient, vdb.values.baseAddress);
const eventRepo = new ReleaseEventRepository(httpClient, urlMapper);
const songListRepo = new SongListRepository(httpClient, urlMapper);
const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);
const tagRepo = new TagRepository(httpClient, vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);

const playQueueRepoFactory = new PlayQueueRepositoryFactory(
	songListRepo,
	songRepo,
	userRepo,
);

export interface VdbPlayerContextProps {
	vdbPlayer: VdbPlayerStore;
	playQueue: PlayQueueStore;
}

export const VdbPlayerContext = React.createContext<VdbPlayerContextProps>(
	undefined!,
);

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
				artistRepo,
				tagRepo,
			),
	);

	return (
		<VdbPlayerContext.Provider
			value={{ vdbPlayer, playQueue: vdbPlayer.playQueue }}
		>
			{children}
		</VdbPlayerContext.Provider>
	);
};

export const useVdbPlayer = (): VdbPlayerContextProps => {
	return React.useContext(VdbPlayerContext);
};
