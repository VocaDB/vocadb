import { ArtistToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ArtistType from '@Models/Artists/ArtistType';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

import ArtistTypeLabel from './ArtistTypeLabel';

interface ArtistLinkProps {
	artist: ArtistContract;
	typeLabel?: boolean;
	name?: string;
	releaseYear?: boolean;
	tooltip?: boolean;
}

const ArtistLink = ({
	artist,
	typeLabel = false,
	name,
	releaseYear = false,
	tooltip = false,
}: ArtistLinkProps): React.ReactElement => {
	return (
		<>
			{typeLabel && (
				<ArtistTypeLabel
					artistType={ArtistType[artist.artistType as keyof typeof ArtistType]}
				/>
			)}{' '}
			{tooltip ? (
				<ArtistToolTip
					as="a"
					href={EntryUrlMapper.details(EntryType.Artist, artist.id)}
					title={artist.additionalNames}
					id={artist.id}
					className="artistLink"
				>
					{name ?? artist.name}
				</ArtistToolTip>
			) : (
				<a
					href={EntryUrlMapper.details(EntryType.Artist, artist.id)}
					title={artist.additionalNames}
					className="artistLink"
				>
					{name ?? artist.name}
				</a>
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

export default ArtistLink;
