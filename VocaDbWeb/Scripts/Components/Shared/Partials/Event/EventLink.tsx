import { EventToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface EventLinkProps {
	event: ReleaseEventContract;
	tooltip?: boolean;
}

export const EventLink = ({
	event,
	tooltip,
}: EventLinkProps): React.ReactElement => {
	return tooltip ? (
		<EventToolTip
			as={Link}
			to={EntryUrlMapper.details(EntryType[EntryType.ReleaseEvent], event.id)}
			id={event.id}
		>
			{event.name}
		</EventToolTip>
	) : (
		<Link
			to={EntryUrlMapper.details(EntryType[EntryType.ReleaseEvent], event.id)}
		>
			{event.name}
		</Link>
	);
};
