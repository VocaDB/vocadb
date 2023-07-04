import { EventToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface EventLinkBaseProps {
	event: ReleaseEventContract;
	bold?: boolean;
}

const EventLinkBase = ({
	event,
	bold,
}: EventLinkBaseProps): React.ReactElement => {
	return (
		<Link
			style={bold ? { fontWeight: 700 } : undefined}
			to={EntryUrlMapper.details(EntryType.ReleaseEvent, event.id)}
		>
			{event.name}
		</Link>
	);
};

interface EventLinkProps {
	event: ReleaseEventContract;
	tooltip?: boolean;
	bold?: boolean;
}

export const EventLink = ({
	event,
	tooltip,
	bold,
}: EventLinkProps): React.ReactElement => {
	return tooltip ? (
		<EventToolTip id={event.id}>
			<EventLinkBase bold={bold} event={event} />
		</EventToolTip>
	) : (
		<EventLinkBase event={event} />
	);
};
