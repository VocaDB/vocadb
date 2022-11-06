import { AlbumThumbItem } from '@/Components/Shared/Partials/Shared/AlbumThumbItem';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import React from 'react';

interface AlbumThumbsProps {
	albums: AlbumForApiContract[];
	tooltip?: boolean;
}

export const AlbumThumbs = React.memo(
	({ albums, tooltip }: AlbumThumbsProps): React.ReactElement => {
		return (
			<div className="smallThumbs">
				{albums.map((album) => (
					<AlbumThumbItem album={album} tooltip={tooltip} key={album.id} />
				))}
			</div>
		);
	},
);
