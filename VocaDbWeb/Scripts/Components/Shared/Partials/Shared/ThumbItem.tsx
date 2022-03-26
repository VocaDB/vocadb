import { BsPrefixRefForwardingComponent } from '@Bootstrap/helpers';
import EntryRefContract from '@DataContracts/EntryRefContract';
import React from 'react';

import { EntryToolTip } from '../../../KnockoutExtensions/EntryToolTip';

interface ThumbItemProps {
	as?: React.ElementType;
	thumbUrl: string;
	caption?: string;
	entry?: EntryRefContract;
	tooltip?: boolean;
}

const ThumbItem: BsPrefixRefForwardingComponent/* TODO */ <
	'a',
	ThumbItemProps
> = ({
	as: Component = 'a',
	thumbUrl,
	caption,
	entry,
	tooltip,
	...props
}: ThumbItemProps): React.ReactElement => {
	return (
		<li>
			<Component {...props}>
				<div className="pictureFrame">
					{entry ? (
						tooltip ? (
							<EntryToolTip
								as="img"
								src={thumbUrl}
								alt="Preview" /* TODO: localize */
								className="coverPic"
								value={entry}
							/>
						) : (
							<img
								src={thumbUrl}
								alt="Preview" /* TODO: localize */
								className="coverPic"
							/>
						)
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
