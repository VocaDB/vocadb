import AlbumSearchList from '@Components/Search/Partials/AlbumSearchList';
import ArtistDetailsContract from '@DataContracts/Artist/ArtistDetailsContract';
import ArtistDetailsStore from '@Stores/Artist/ArtistDetailsStore';
import { useStoreWithPagination } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useParams } from 'react-router';

import { AlbumOptions } from './ArtistDetails';
import { ArtistDetailsTabs } from './ArtistDetailsRoutes';

interface ArtistCollaborationAlbumsProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistCollaborationAlbums = observer(
	({
		artist,
		artistDetailsStore,
	}: ArtistCollaborationAlbumsProps): React.ReactElement => {
		const { id } = useParams();

		React.useEffect(() => {
			artistDetailsStore.collaborationAlbumsStore.artistFilters.artistIds = [
				Number(id),
			];
		}, [id, artistDetailsStore]);

		useStoreWithPagination(artistDetailsStore.collaborationAlbumsStore);

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="collaborations"
			>
				<AlbumOptions
					albumSearchStore={artistDetailsStore.collaborationAlbumsStore}
				/>
				<div>
					<AlbumSearchList
						albumSearchStore={artistDetailsStore.collaborationAlbumsStore}
					/>
				</div>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistCollaborationAlbums;
