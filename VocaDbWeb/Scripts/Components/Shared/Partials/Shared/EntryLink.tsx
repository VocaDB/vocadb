import EntryBaseContract from '@DataContracts/EntryBaseContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface EntryLinkProps {
	entry: EntryBaseContract;
}

const EntryLink = React.memo(
	({ entry }: EntryLinkProps): React.ReactElement => {
		return (
			<a href={EntryUrlMapper.details_entry(entry)}>{entry.defaultName}</a>
		);
	},
);

export default EntryLink;
