import { VenueLink } from '@/Components/Shared/Partials/Venue/VenueLink';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import React from 'react';

interface VenueLinkOrVenueNameProps {
	event: ReleaseEventContract;
}

export const VenueLinkOrVenueName = React.memo(
	({ event }: VenueLinkOrVenueNameProps): React.ReactElement => {
		return event.venue ? (
			<VenueLink venue={event.venue} />
		) : (
			<>{event.venueName}</>
		);
	},
);
