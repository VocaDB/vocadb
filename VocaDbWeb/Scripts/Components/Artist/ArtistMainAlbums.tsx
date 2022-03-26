import AlbumSearchList from '@Components/Search/Partials/AlbumSearchList';
import useStoreWithPaging from '@Components/useStoreWithPaging';
import ArtistDetailsContract from '@DataContracts/Artist/ArtistDetailsContract';
import ArtistDetailsStore from '@Stores/Artist/ArtistDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useParams } from 'react-router';

import { AlbumOptions } from './ArtistDetails';
import { ArtistDetailsTabs } from './ArtistDetailsRoutes';

interface ArtistMainAlbumsProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistMainAlbums = observer(
	({
		artist,
		artistDetailsStore,
	}: ArtistMainAlbumsProps): React.ReactElement => {
		const { id } = useParams();

		React.useEffect(() => {
			artistDetailsStore.mainAlbumsStore.artistFilters.artistIds = [Number(id)];
		}, [id, artistDetailsStore]);

		useStoreWithPaging(artistDetailsStore.mainAlbumsStore);

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="albums"
			>
				<AlbumOptions albumSearchStore={artistDetailsStore.mainAlbumsStore} />
				<div>
					<AlbumSearchList
						albumSearchStore={artistDetailsStore.mainAlbumsStore}
					/>
				</div>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistMainAlbums;
