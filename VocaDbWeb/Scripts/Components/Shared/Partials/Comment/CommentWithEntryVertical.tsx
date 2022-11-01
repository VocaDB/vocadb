import { CommentEntryItem } from '@/Components/Shared/Partials/Comment/CommentEntryItem';
import { PrintComment } from '@/Components/Shared/Partials/Comment/PrintComment';
import { EntryWithCommentsContract } from '@/DataContracts/EntryWithCommentsContract';
import { useMutedUsers } from '@/MutedUsersContext';
import { uniq } from 'lodash-es';
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
		const authorIds = uniq(entry.comments.map((comment) => comment.author.id));

		const mutedUsers = useMutedUsers();
		if (authorIds.length === 1 && mutedUsers.includes(authorIds[0]))
			return <></>;

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
