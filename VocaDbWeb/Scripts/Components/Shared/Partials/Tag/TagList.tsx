import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import React from 'react';

import TagLink from './TagLink';

interface TagListProps {
	tagNames: TagBaseContract[];
	tooltip?: boolean;
}

const TagList = React.memo(
	({ tagNames, tooltip = false }: TagListProps): React.ReactElement => {
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

export default TagList;
