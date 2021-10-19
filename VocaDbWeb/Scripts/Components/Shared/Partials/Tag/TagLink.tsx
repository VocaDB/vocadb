import { TagToolTip } from '@Components/KnockoutExtensions/EntryToolTip';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface TagLinkProps {
	tag: TagBaseContract;
	tooltip?: boolean;
}

const TagLink = React.memo(
	({ tag, tooltip }: TagLinkProps): React.ReactElement => {
		return tooltip ? (
			<TagToolTip
				as={Link}
				to={EntryUrlMapper.details_tag_contract(tag)}
				title={tag.additionalNames}
				id={tag.id}
			>
				{tag.name}
			</TagToolTip>
		) : (
			<Link
				to={EntryUrlMapper.details_tag_contract(tag)}
				title={tag.additionalNames}
			>
				{tag.name}
			</Link>
		);
	},
);

export default TagLink;
