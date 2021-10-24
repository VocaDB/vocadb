import EntryContract from '@DataContracts/EntryContract';
import UrlHelper from '@Helpers/UrlHelper';
import ImageSize from '@Models/Images/ImageSize';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface CommentEntryItemProps {
	entry: EntryContract;
}

const CommentEntryItem = React.memo(
	({ entry }: CommentEntryItemProps): React.ReactElement => {
		const thumbUrl = entry.mainPicture
			? UrlHelper.upgradeToHttps(
					UrlHelper.getSmallestThumb(entry.mainPicture, ImageSize.TinyThumb),
			  )
			: undefined;

		return (
			<div className="media">
				{thumbUrl && (
					<a
						className="pull-left"
						href={EntryUrlMapper.details_entry(entry)}
						title={entry.additionalNames}
					>
						<img
							src={thumbUrl}
							alt="thumb" /* TODO: localize */
							className="coverPicThumb"
							referrerPolicy="same-origin"
						/>
					</a>
				)}

				<div className="media-body">
					<h4 className="media-heading">
						<a
							href={EntryUrlMapper.details_entry(entry)}
							title={entry.additionalNames}
						>
							{entry.name}
						</a>
					</h4>
					{entry.artistString && <span>{entry.artistString}</span>}
				</div>
			</div>
		);
	},
);

export default CommentEntryItem;
