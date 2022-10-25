import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { truncate } from 'lodash-es';
import React from 'react';

interface TagPopupContentProps {
	tag: TagApiContract;
}

export const TagPopupContent = React.memo(
	({ tag }: TagPopupContentProps): React.ReactElement => {
		return (
			<>
				{tag.mainPicture && tag.mainPicture.urlSmallThumb && (
					<div className="thumbnail">
						<img
							src={tag.mainPicture.urlSmallThumb}
							alt="Thumb" /* TODO: localize */
							className="coverPicThumb"
							referrerPolicy="same-origin"
						/>
					</div>
				)}

				<strong className="popupTitle">{tag.name}</strong>

				{tag.additionalNames && <p>{tag.additionalNames}</p>}

				{tag.categoryName && <p>{tag.categoryName}</p>}

				{tag.description && (
					<p>
						<Markdown>
							{truncate(tag.description, {
								length: 100,
							})}
						</Markdown>
					</p>
				)}
			</>
		);
	},
);
