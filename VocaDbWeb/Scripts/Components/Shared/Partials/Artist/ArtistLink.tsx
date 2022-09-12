import { ArtistTypeLabel } from '@/Components/Shared/Partials/Artist/ArtistTypeLabel';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface ArtistLinkBaseProps {
	artist: ArtistContract;
	children?: React.ReactNode;
}

const ArtistLinkBase = ({
	artist,
	children,
}: ArtistLinkBaseProps): React.ReactElement => {
	return (
		<Link
			to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
			title={artist.additionalNames}
			className="artistLink"
		>
			{children ?? artist.name}
		</Link>
	);
};

interface ArtistLinkProps {
	artist: ArtistContract;
	typeLabel?: boolean;
	name?: string;
	releaseYear?: boolean;
	tooltip?: boolean;
}

export const ArtistLink = ({
	artist,
	typeLabel = false,
	name,
	releaseYear = false,
	tooltip = false,
}: ArtistLinkProps): React.ReactElement => {
	return (
		<>
			{typeLabel && <ArtistTypeLabel artistType={artist.artistType} />}
			{typeLabel && ' '}
			{tooltip ? (
				/*<ArtistToolTip
					as={Link}
					to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
					title={artist.additionalNames}
					id={artist.id}
					className="artistLink"
				>
					{name ?? artist.name}
				</ArtistToolTip>*/
				<ArtistLinkBase artist={artist}>{name}</ArtistLinkBase>
			) : (
				<ArtistLinkBase artist={artist}>{name}</ArtistLinkBase>
			)}
			{releaseYear && artist.releaseDate && (
				<>
					{' '}
					<small className="muted">
						({new Date(artist.releaseDate).getFullYear() /* REVIEW */})
					</small>
				</>
			)}
		</>
	);
};
