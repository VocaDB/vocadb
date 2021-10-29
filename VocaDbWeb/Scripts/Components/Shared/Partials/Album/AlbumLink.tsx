import { AlbumToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import AlbumForApiContract from '@DataContracts/Album/AlbumForApiContract';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface AlbumLinkProps {
	album: AlbumForApiContract;
	tooltip: boolean;
}

const AlbumLink = ({
	album,
	tooltip = false,
}: AlbumLinkProps): React.ReactElement => {
	return tooltip ? (
		<AlbumToolTip
			as={'a'}
			href={EntryUrlMapper.details(EntryType.Album, album.id)}
			title={album.additionalNames}
			id={album.id}
			className="albumLink"
		>
			{album.name}
		</AlbumToolTip>
	) : (
		<a
			href={`${EntryUrlMapper.details(EntryType.Album, album.id)}`}
			title={album.additionalNames}
			className="albumLink"
		>
			{album.name}
		</a>
	);
};

export default AlbumLink;
