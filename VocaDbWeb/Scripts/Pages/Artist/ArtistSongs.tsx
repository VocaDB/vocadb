import Button from '@/Bootstrap/Button';
import ButtonGroup from '@/Bootstrap/ButtonGroup';
import { SongSearchDropdown } from '@/Components/Shared/Partials/Knockout/SearchDropdown';
import { useVdbPlayer } from '@/Components/VdbPlayer/VdbPlayerContext';
import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import SongSearchList from '@/Pages/Search/Partials/SongSearchList';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import {
	AutoplayContext,
	PlayQueueRepositoryType,
} from '@/Stores/VdbPlayer/PlayQueueStore';
import { useStoreWithUpdateResults } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useParams } from 'react-router';

interface ArtistSongsProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistSongs = observer(
	({ artist, artistDetailsStore }: ArtistSongsProps): React.ReactElement => {
		const { id } = useParams();

		const songsStore = artistDetailsStore.songsStore;

		React.useEffect(() => {
			songsStore.artistFilters.artistIds = [Number(id)];
		}, [id, songsStore]);

		useStoreWithUpdateResults(songsStore);

		const { playQueue } = useVdbPlayer();

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="songs"
			>
				<div className="clearfix">
					<div className="pull-right">
						<SongSearchDropdown songSearchStore={songsStore} />{' '}
						<ButtonGroup>
							<Button
								onClick={async (): Promise<void> => {
									await playQueue.startAutoplay(
										new AutoplayContext(
											PlayQueueRepositoryType.Songs,
											songsStore.queryParams,
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
					<SongSearchList songSearchStore={songsStore} />
				</div>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistSongs;
