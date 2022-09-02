import { ThumbItem } from '@/Components/Shared/Partials/Shared/ThumbItem';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { EntryType } from '@/Models/EntryType';
import { ImageSize } from '@/Models/Images/ImageSize';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface AlbumThumbItemProps {
	album: AlbumForApiContract;
	tooltip?: boolean;
}

const AlbumThumbItem = React.memo(
	({ album, tooltip }: AlbumThumbItemProps): React.ReactElement => {
		return (
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
			/>
		);
	},
);

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
