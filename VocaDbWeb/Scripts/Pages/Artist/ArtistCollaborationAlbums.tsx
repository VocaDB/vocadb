import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { AlbumOptions } from '@/Pages/Artist/ArtistDetails';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import AlbumSearchList from '@/Pages/Search/Partials/AlbumSearchList';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useParams } from 'react-router';

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

		useLocationStateStore(artistDetailsStore.collaborationAlbumsStore);

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
