import AlbumForApiContract from '@DataContracts/Album/AlbumForApiContract';
import UrlHelper from '@Helpers/UrlHelper';
import EntryType from '@Models/EntryType';
import ImageSize from '@Models/Images/ImageSize';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface AlbumIconLinkProps {
	album: AlbumForApiContract;
}

const AlbumIconLink = ({ album }: AlbumIconLinkProps): React.ReactElement => {
	return (
		<a href={EntryUrlMapper.details(EntryType.Album, album.id)}>
			<img
				src={UrlHelper.imageThumb(album.mainPicture, ImageSize.TinyThumb)}
				alt="Cover" /* TODO: localize */
				className="coverPicThumb"
			/>
		</a>
	);
};

export default AlbumIconLink;
