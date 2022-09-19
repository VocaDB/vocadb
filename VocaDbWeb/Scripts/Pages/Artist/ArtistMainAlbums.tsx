import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { AlbumOptions } from '@/Pages/Artist/ArtistDetails';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import AlbumSearchList from '@/Pages/Search/Partials/AlbumSearchList';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useParams } from 'react-router';

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

		useLocationStateStore(artistDetailsStore.mainAlbumsStore);

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
