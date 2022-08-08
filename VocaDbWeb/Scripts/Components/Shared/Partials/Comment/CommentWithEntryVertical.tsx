import CommentEntryItem from '@/Components/Shared/Partials/Comment/CommentEntryItem';
import PrintComment from '@/Components/Shared/Partials/Comment/PrintComment';
import EntryWithCommentsContract from '@/DataContracts/EntryWithCommentsContract';
import React from 'react';

interface CommentWithEntryVerticalProps {
	entry: EntryWithCommentsContract;
	maxLength?: number;
}

const CommentWithEntryVertical = React.memo(
	({
		entry,
		maxLength = 2147483647,
	}: CommentWithEntryVerticalProps): React.ReactElement => {
		return (
			<div className="well well-transparent">
				<CommentEntryItem entry={entry.entry} />
				{entry.comments.map((comment, index) => (
					<React.Fragment key={comment.id}>
						{index > 0 && <hr />}
						<PrintComment
							contract={comment}
							allowDelete={false}
							maxLength={maxLength}
						/>
					</React.Fragment>
				))}
			</div>
		);
	},
);

export default CommentWithEntryVertical;
