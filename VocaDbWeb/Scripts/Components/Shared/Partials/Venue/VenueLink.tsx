import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface VenueLinkProps {
	venue: VenueForApiContract;
}

const VenueLink = React.memo(
	({ venue }: VenueLinkProps): React.ReactElement => {
		return (
			<a
				href={EntryUrlMapper.details(EntryType.Venue, venue.id)}
				title={venue.additionalNames}
			>
				{venue.name}
			</a>
		);
	},
);

export default VenueLink;
