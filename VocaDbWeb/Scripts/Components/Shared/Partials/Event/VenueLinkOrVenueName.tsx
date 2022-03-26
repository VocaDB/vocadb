import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import React from 'react';

import VenueLink from '../Venue/VenueLink';

interface VenueLinkOrVenueNameProps {
	event: ReleaseEventContract;
}

const VenueLinkOrVenueName = React.memo(
	({ event }: VenueLinkOrVenueNameProps): React.ReactElement => {
		return event.venue ? (
			<VenueLink venue={event.venue} />
		) : (
			<>{event.venueName}</>
		);
	},
);

export default VenueLinkOrVenueName;
