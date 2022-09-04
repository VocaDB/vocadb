import { AlbumToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface AlbumLinkBaseProps {
	album: AlbumForApiContract;
}

const AlbumLinkBase = ({ album }: AlbumLinkBaseProps): React.ReactElement => {
	return (
		<Link
			to={`${EntryUrlMapper.details(EntryType.Album, album.id)}`}
			title={album.additionalNames}
			className="albumLink"
		>
			{album.name}
		</Link>
	);
};

interface AlbumLinkProps {
	album: AlbumForApiContract;
	tooltip: boolean;
}

export const AlbumLink = ({
	album,
	tooltip = false,
}: AlbumLinkProps): React.ReactElement => {
	return tooltip ? (
		<AlbumToolTip id={album.id}>
			<AlbumLinkBase album={album} />
		</AlbumToolTip>
	) : (
		<AlbumLinkBase album={album} />
	);
};
