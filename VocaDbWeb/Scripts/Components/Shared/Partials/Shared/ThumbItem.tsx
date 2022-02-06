import { BsPrefixRefForwardingComponent } from '@Bootstrap/helpers';
import EntryRefContract from '@DataContracts/EntryRefContract';
import React from 'react';

interface ThumbItemProps {
	as?: React.ElementType;
	thumbUrl: string;
	caption?: string;
	entry?: EntryRefContract;
}

const ThumbItem: BsPrefixRefForwardingComponent/* TODO */ <
	'a',
	ThumbItemProps
> = ({
	as: Component = 'a',
	thumbUrl,
	caption,
	entry,
	...props
}: ThumbItemProps): React.ReactElement => {
	return (
		<li>
			<Component {...props}>
				<div className="pictureFrame">
					{entry ? (
						<img
							src={thumbUrl}
							alt="Preview" /* TODO: localize */
							className="coverPic"
							data-entry-type={entry.entryType}
							data-entry-id={entry.id}
						/>
					) : (
						<img
							src={thumbUrl}
							alt="Preview" /* TODO: localize */
							className="coverPic"
						/>
					)}
				</div>
			</Component>
			{caption && <p>{caption}</p>}
		</li>
	);
};

export default ThumbItem;
