import { ArtistToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

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
			{typeLabel && <ArtistTypeLabel artistType={artist.artistType} />}
			{typeLabel && ' '}
			{tooltip ? (
				<ArtistToolTip
					as={Link}
					to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
					title={artist.additionalNames}
					id={artist.id}
					className="artistLink"
				>
					{name ?? artist.name}
				</ArtistToolTip>
			) : (
				<Link
					to={EntryUrlMapper.details(EntryType.Artist, artist.id)}
					title={artist.additionalNames}
					className="artistLink"
				>
					{name ?? artist.name}
				</Link>
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
