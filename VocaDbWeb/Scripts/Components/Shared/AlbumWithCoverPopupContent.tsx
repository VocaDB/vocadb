import { AlbumPopupContent } from '@/Components/Shared/AlbumPopupContent';
import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import React from 'react';

interface AlbumWithCoverPopupContentProps {
	album: AlbumContract;
}

export const AlbumWithCoverPopupContent = ({
	album,
}: AlbumWithCoverPopupContentProps): React.ReactElement => {
	return (
		<>
			<img
				src={UrlHelper.imageThumb(album.mainPicture, ImageSize.SmallThumb)}
				alt="Cover" /* LOC */
				className="smallCover"
			/>
			<br />
			<br />
			<AlbumPopupContent album={album} />
		</>
	);
};
