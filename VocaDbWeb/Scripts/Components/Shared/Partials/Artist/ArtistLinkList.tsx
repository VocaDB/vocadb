import ArtistApiContract from '@DataContracts/Artist/ArtistApiContract';
import React from 'react';

import ArtistLink from './ArtistLink';

interface ArtistLinkListProps {
	artists: ArtistApiContract[];
	typeLabel?: boolean;
	releaseYear?: boolean;
	tooltip?: boolean;
}

const ArtistLinkList = React.memo(
	({
		artists,
		typeLabel = false,
		releaseYear = false,
		tooltip = false,
	}: ArtistLinkListProps): React.ReactElement => {
		return (
			<>
				{artists.map((artist, index) => (
					<React.Fragment key={artist.id}>
						{index > 0 && ', '}
						<ArtistLink
							artist={artist}
							typeLabel={typeLabel}
							releaseYear={releaseYear}
							tooltip={tooltip}
						/>
					</React.Fragment>
				))}
			</>
		);
	},
);

export default ArtistLinkList;
