import { EventToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { EntryType } from '@/Models/EntryType';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import dayjs from 'dayjs';
import React from 'react';
import { Link } from 'react-router-dom';

interface EventLinkBaseProps {
	event: ReleaseEventContract;
	bold?: boolean;
	entryReleaseDate?: string;
}

const EventLinkBase = ({
	event,
	bold,
	entryReleaseDate,
}: EventLinkBaseProps): React.ReactElement => {
	let eventDelta;
	if (entryReleaseDate !== undefined && event.date !== undefined) {
		let endEventDelta =
			event.endDate !== undefined
				? Math.floor(dayjs(entryReleaseDate).diff(event.endDate, 'days', true))
				: Infinity;
		eventDelta = Math.ceil(
			dayjs(entryReleaseDate).diff(event.date, 'days', true),
		);
		if (Math.abs(endEventDelta) < Math.abs(eventDelta)) {
			eventDelta = endEventDelta;
		}
	}

	return (
		<>
			<Link
				style={bold ? { fontWeight: 700 } : undefined}
				to={EntryUrlMapper.details(EntryType.ReleaseEvent, event.id)}
			>
				{event.name}
			</Link>
			{eventDelta !== undefined &&
				Math.abs(eventDelta) > 7 &&
				` (${eventDelta > 0 ? '+' : ''}${eventDelta} days)`}
		</>
	);
};

interface EventLinkProps {
	event: ReleaseEventContract;
	tooltip?: boolean;
	bold?: boolean;
	entryReleaseDate?: string;
}

export const EventLink = ({
	event,
	tooltip,
	bold,
	entryReleaseDate,
}: EventLinkProps): React.ReactElement => {
	console.log(entryReleaseDate);
	return tooltip ? (
		<EventToolTip id={event.id}>
			<EventLinkBase
				bold={bold}
				event={event}
				entryReleaseDate={entryReleaseDate}
			/>
		</EventToolTip>
	) : (
		<EventLinkBase event={event} entryReleaseDate={entryReleaseDate} />
	);
};
