import { EntryContract } from '@/DataContracts/EntryContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface CommentEntryItemProps {
	entry: EntryContract;
}

export const CommentEntryItem = React.memo(
	({ entry }: CommentEntryItemProps): React.ReactElement => {
		const thumbUrl = entry.mainPicture
			? UrlHelper.upgradeToHttps(
					UrlHelper.getSmallestThumb(entry.mainPicture, ImageSize.TinyThumb),
			  )
			: undefined;

		return (
			<div className="media">
				{thumbUrl && (
					<Link
						className="pull-left"
						to={EntryUrlMapper.details_entry(entry)}
						title={entry.additionalNames}
					>
						<img
							src={thumbUrl}
							alt="thumb" /* LOCALIZE */
							className="coverPicThumb"
							referrerPolicy="same-origin"
						/>
					</Link>
				)}

				<div className="media-body">
					<h4 className="media-heading">
						<Link
							to={EntryUrlMapper.details_entry(entry)}
							title={entry.additionalNames}
						>
							{entry.name}
						</Link>
					</h4>
					{entry.artistString && <span>{entry.artistString}</span>}
				</div>
			</div>
		);
	},
);
