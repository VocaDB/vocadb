import { TagToolTip } from '@/Components/KnockoutExtensions/EntryToolTip';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link, LinkProps } from 'react-router-dom';

interface TagLinkBaseProps extends Omit<LinkProps, 'to'> {
	tag: TagBaseContract;
	children?: React.ReactNode;
}

const TagLinkBase = ({
	tag,
	children,
	...props
}: TagLinkBaseProps): React.ReactElement => {
	return (
		<Link {...props} to={EntryUrlMapper.details_tag_contract(tag)!}>
			{children ?? tag.name}
		</Link>
	);
};

interface TagLinkProps {
	tag: TagBaseContract;
	children?: React.ReactNode;
	tooltip?: boolean;
}

export const TagLink = React.memo(
	({ tag, children, tooltip }: TagLinkProps): React.ReactElement => {
		return tooltip ? (
			<TagToolTip id={tag.id}>
				<TagLinkBase tag={tag}>{children}</TagLinkBase>
			</TagToolTip>
		) : (
			<TagLinkBase tag={tag} title={tag.additionalNames}>
				{children}
			</TagLinkBase>
		);
	},
);
