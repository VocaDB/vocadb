import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface AlbumIconLinkProps {
	album: AlbumForApiContract;
}

export const AlbumIconLink = ({
	album,
}: AlbumIconLinkProps): React.ReactElement => {
	return (
		<Link to={EntryUrlMapper.details(EntryType.Album, album.id)}>
			<img
				src={UrlHelper.imageThumb(album.mainPicture, ImageSize.TinyThumb)}
				alt="Cover" /* TODO: localize */
				className="coverPicThumb"
			/>
		</Link>
	);
};
