import { CommentEntryItem } from '@/Components/Shared/Partials/Comment/CommentEntryItem';
import { PrintComment } from '@/Components/Shared/Partials/Comment/PrintComment';
import { EntryWithCommentsContract } from '@/DataContracts/EntryWithCommentsContract';
import { useMutedUsers } from '@/MutedUsersContext';
import React from 'react';

interface CommentWithEntryVerticalProps {
	entry: EntryWithCommentsContract;
	maxLength?: number;
}

export const CommentWithEntryVertical = React.memo(
	({
		entry,
		maxLength = 2147483647,
	}: CommentWithEntryVerticalProps): React.ReactElement => {
		const mutedUsers = useMutedUsers();
		if (
			entry.comments.every((comment) => mutedUsers.includes(comment.author.id))
		) {
			return <></>;
		}

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
