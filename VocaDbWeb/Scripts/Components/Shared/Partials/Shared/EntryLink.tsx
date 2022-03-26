import EntryBaseContract from '@DataContracts/EntryBaseContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface EntryLinkProps {
	entry: EntryBaseContract;
}

const EntryLink = React.memo(
	({ entry }: EntryLinkProps): React.ReactElement => {
		return (
			<Link to={EntryUrlMapper.details_entry(entry)}>{entry.defaultName}</Link>
		);
	},
);

export default EntryLink;
