import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import React from 'react';

import TagLink from './TagLink';

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
