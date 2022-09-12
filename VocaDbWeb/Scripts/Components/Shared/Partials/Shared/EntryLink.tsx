import { EntryToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { EntryBaseContract } from '@/DataContracts/EntryBaseContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link, LinkProps } from 'react-router-dom';

interface EntryLinkProps extends Omit<LinkProps, 'to'> {
	entry: EntryBaseContract;
	children?: React.ReactNode;
	tooltip?: boolean;
}

export const EntryLink = React.memo(
	({
		entry,
		children,
		tooltip,
		...props
	}: EntryLinkProps): React.ReactElement => {
		return tooltip ? (
			<EntryToolTip
				{...props}
				as={Link}
				value={entry}
				to={EntryUrlMapper.details_entry(entry)}
			>
				{children ?? entry.defaultName}
			</EntryToolTip>
		) : (
			<Link {...props} to={EntryUrlMapper.details_entry(entry)}>
				{children ?? entry.defaultName}
			</Link>
		);
	},
);
