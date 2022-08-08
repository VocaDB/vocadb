import TagLink from '@/Components/Shared/Partials/Tag/TagLink';
import TagBaseContract from '@/DataContracts/Tag/TagBaseContract';
import React from 'react';

interface TagLinkListProps {
	tagNames: TagBaseContract[];
	tooltip?: boolean;
}

const TagLinkList = React.memo(
	({ tagNames, tooltip = false }: TagLinkListProps): React.ReactElement => {
		return (
			<>
				{tagNames.map((tagName, index) => (
					<React.Fragment key={tagName.id}>
						{index > 0 && ', '}
						<TagLink tag={tagName} tooltip={tooltip} />
					</React.Fragment>
				))}
			</>
		);
	},
);

export default TagLinkList;
