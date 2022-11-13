import { ArtistToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { ArtistTypeLabel } from '@/Components/Shared/Partials/Artist/ArtistTypeLabel';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link, LinkProps } from 'react-router-dom';

interface ArtistLinkBaseProps extends Omit<LinkProps, 'to'> {
	artist: ArtistContract;
	children?: React.ReactNode;
}

const ArtistLinkBase = ({
	artist,
	children,
	...props
}: ArtistLinkBaseProps): React.ReactElement => {
	return (
		<Link
			{...props}
			to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
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
				<ArtistToolTip id={artist.id}>
					<ArtistLinkBase artist={artist}>{name}</ArtistLinkBase>
				</ArtistToolTip>
			) : (
				<ArtistLinkBase artist={artist} title={artist.additionalNames}>
					{name}
				</ArtistLinkBase>
			)}
			{releaseYear && artist.releaseDate && (
				<>
					{' '}
					<small className="muted">
						({new Date(artist.releaseDate).getUTCFullYear() /* REVIEW */})
					</small>
				</>
			)}
		</>
	);
};
