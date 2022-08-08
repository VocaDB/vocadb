import ThumbItem from '@/Components/Shared/Partials/Shared/ThumbItem';
import AlbumForApiContract from '@/DataContracts/Album/AlbumForApiContract';
import UrlHelper from '@/Helpers/UrlHelper';
import EntryType from '@/Models/EntryType';
import ImageSize from '@/Models/Images/ImageSize';
import EntryUrlMapper from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface AlbumThumbsProps {
	albums: AlbumForApiContract[];
	tooltip?: boolean;
}

const AlbumThumbs = ({
	albums,
	tooltip,
}: AlbumThumbsProps): React.ReactElement => {
	return (
		<ul className="smallThumbs">
			{albums.map((album) => (
				<ThumbItem
					as={Link}
					to={EntryUrlMapper.details(EntryType.Album, album.id)}
					thumbUrl={
						UrlHelper.imageThumb(album.mainPicture, ImageSize.SmallThumb) ??
						'/Content/unknown.png'
					}
					caption={album.name}
					entry={{ entryType: EntryType[EntryType.Album], id: album.id }}
					tooltip={tooltip}
					key={album.id}
				/>
			))}
		</ul>
	);
};

export default AlbumThumbs;
