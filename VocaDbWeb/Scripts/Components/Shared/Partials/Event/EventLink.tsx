import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface EventLinkBaseProps {
	event: ReleaseEventContract;
}

const EventLinkBase = ({ event }: EventLinkBaseProps): React.ReactElement => {
	return (
		<Link
			to={EntryUrlMapper.details(EntryType[EntryType.ReleaseEvent], event.id)}
		>
			{event.name}
		</Link>
	);
};

interface EventLinkProps {
	event: ReleaseEventContract;
	tooltip?: boolean;
}

export const EventLink = ({
	event,
	tooltip,
}: EventLinkProps): React.ReactElement => {
	return tooltip ? (
		/*<EventToolTip
			as={Link}
			to={EntryUrlMapper.details(EntryType[EntryType.ReleaseEvent], event.id)}
			id={event.id}
		>
			{event.name}
		</EventToolTip>*/
		<EventLinkBase event={event} />
	) : (
		<EventLinkBase event={event} />
	);
};
