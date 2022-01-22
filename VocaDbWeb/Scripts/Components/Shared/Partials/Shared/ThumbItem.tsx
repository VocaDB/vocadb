import EntryRefContract from '@DataContracts/EntryRefContract';
import React from 'react';

interface ThumbItemProps {
	url: string;
	thumbUrl: string;
	caption?: string;
	entry?: EntryRefContract;
}

const ThumbItem = ({
	url,
	thumbUrl,
	caption,
	entry,
}: ThumbItemProps): React.ReactElement => {
	return (
		<li>
			<a href={url}>
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
			</a>
			{caption && <p>{caption}</p>}
		</li>
	);
};

export default ThumbItem;
