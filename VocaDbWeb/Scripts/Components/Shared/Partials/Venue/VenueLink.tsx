import VenueForApiContract from '@DataContracts/Venue/VenueForApiContract';
import EntryType from '@Models/EntryType';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface VenueLinkProps {
	venue: VenueForApiContract;
}

const VenueLink = React.memo(
	({ venue }: VenueLinkProps): React.ReactElement => {
		return (
			<Link
				to={EntryUrlMapper.details(EntryType.Venue, venue.id)}
				title={venue.additionalNames}
			>
				{venue.name}
			</Link>
		);
	},
);

export default VenueLink;
