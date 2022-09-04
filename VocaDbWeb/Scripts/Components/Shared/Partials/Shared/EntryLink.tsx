import { EntryToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { EntryBaseContract } from '@/DataContracts/EntryBaseContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link, LinkProps } from 'react-router-dom';

interface EntryLinkBaseProps extends Omit<LinkProps, 'to'> {
	entry: EntryBaseContract;
	children?: React.ReactNode;
}

const EntryLinkBase = ({
	entry,
	children,
	...props
}: EntryLinkBaseProps): React.ReactElement => {
	return (
		<Link {...props} to={EntryUrlMapper.details_entry(entry)}>
			{children ?? entry.defaultName}
		</Link>
	);
};

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
			<EntryToolTip entry={entry}>
				<EntryLinkBase {...props} entry={entry}>
					{children}
				</EntryLinkBase>
			</EntryToolTip>
		) : (
			<EntryLinkBase {...props} entry={entry}>
				{children}
			</EntryLinkBase>
		);
	},
);
