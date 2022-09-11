import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import { SongSearchDropdown } from '@/Components/Shared/Partials/Knockout/SearchDropdown';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { VideoServiceHelper } from '@/Helpers/VideoServiceHelper';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import SongSearchList from '@/Pages/Search/Partials/SongSearchList';
import { SongRepository } from '@/Repositories/SongRepository';
import { HttpClient } from '@/Shared/HttpClient';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import { PlayQueueRepositoryForSongsAdapter } from '@/Stores/VdbPlayer/PlayQueueRepositoryForSongsAdapter';
import { useStoreWithPagination } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useParams } from 'react-router';

const httpClient = new HttpClient();

const songRepo = new SongRepository(httpClient, vdb.values.baseAddress);

const playQueueRepo = new PlayQueueRepositoryForSongsAdapter(songRepo);

interface ArtistSongsProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistSongs = observer(
	({ artist, artistDetailsStore }: ArtistSongsProps): React.ReactElement => {
		const { id } = useParams();

		React.useEffect(() => {
			artistDetailsStore.songsStore.artistFilters.artistIds = [Number(id)];
		}, [id, artistDetailsStore]);

		useStoreWithPagination(artistDetailsStore.songsStore);

		const { playQueue } = useVdbPlayer();

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="songs"
			>
				<div className="clearfix">
					<div className="pull-right">
						<SongSearchDropdown
							songSearchStore={artistDetailsStore.songsStore}
						/>{' '}
						<ButtonGroup>
							<Button
								onClick={async (): Promise<void> => {
									// Access queryParams here, not in the function body.
									const { queryParams } = artistDetailsStore.songsStore;

									await playQueue.startAutoplay((offset, limit) =>
										playQueueRepo.getItems(
											VideoServiceHelper.autoplayServices,
											{ getTotalCount: true, maxEntries: limit, start: offset },
											queryParams,
										),
									);
								}}
								title="Play" /* TODO: localize */
								className="btn-nomargin"
							>
								<i className="icon-play noMargin" /> Play{/* TODO: localize */}
							</Button>
						</ButtonGroup>
					</div>
				</div>
				<div>
					<SongSearchList songSearchStore={artistDetailsStore.songsStore} />
				</div>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistSongs;
