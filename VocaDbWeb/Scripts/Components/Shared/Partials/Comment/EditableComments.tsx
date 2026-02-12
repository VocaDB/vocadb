import Alert from '@/Bootstrap/Alert';
import { CommentBodyEditableKnockout } from '@/Components/Shared/Partials/Comment/CommentBodyEditableKnockout';
import { CommentKnockout } from '@/Components/Shared/Partials/Comment/CommentKnockout';
import { CreateComment } from '@/Components/Shared/Partials/Comment/CreateComment';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { CommentStore } from '@/Stores/CommentStore';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import classNames from 'classnames';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EditableCommentsProps {
	editableCommentsStore: EditableCommentsStore;
	allowCreateComment: boolean;
	well?: boolean;
	comments: CommentStore[];
	newCommentRows?: number;
	commentBoxEnd?: boolean;
	pagination?: boolean;
	commentsLocked?: boolean;
	onToggleLock?: () => void;
}

export const EditableComments = observer(
	({
		editableCommentsStore,
		allowCreateComment,
		well = true,
		comments,
		newCommentRows = 6,
		commentBoxEnd = false,
		pagination = true,
		commentsLocked = false,
		onToggleLock,
	}: EditableCommentsProps): React.ReactElement => {
		const loginManager = useLoginManager();
		const className = well ? 'well well-transparent' : 'standalone';
		const { t } = useTranslation(['ViewRes']);

		return (
			<>
				{commentsLocked && !commentBoxEnd && !loginManager.canLockComments && (
					<Alert>{t('ViewRes:DiscussionContent.LockedBanner')}</Alert>
				)}

				{allowCreateComment && !commentBoxEnd && (
					<CreateComment
						editableCommentsStore={editableCommentsStore}
						className={className}
						newCommentRows={newCommentRows}
						commentsLocked={commentsLocked}
						onToggleLock={onToggleLock}
					/>
				)}

				{pagination && editableCommentsStore.paging.hasMultiplePages && (
					<ServerSidePaging pagingStore={editableCommentsStore.paging} />
				)}

				<div>
					{comments.map((comment) => (
						<div
							className={classNames('editable-comment', className)}
							key={comment.id}
						>
							<CommentKnockout
								commentKnockoutStore={comment}
								message={comment.message}
								allowMarkdown={false}
								onDelete={(): void =>
									editableCommentsStore.deleteComment(comment)
								}
								onEdit={(): void =>
									editableCommentsStore.beginEditComment(comment)
								}
								standalone={!well}
							>
								<CommentBodyEditableKnockout
									editableCommentsStore={editableCommentsStore}
									commentStore={comment}
									message={comment.message}
								/>
							</CommentKnockout>
						</div>
					))}
				</div>

				{pagination && editableCommentsStore.paging.hasMultiplePages && (
					<ServerSidePaging pagingStore={editableCommentsStore.paging} />
				)}

				{commentsLocked && commentBoxEnd && !loginManager.canLockComments && (
					<Alert>{t('ViewRes:DiscussionContent.LockedBanner')}</Alert>
				)}

				{allowCreateComment && commentBoxEnd && (
					<CreateComment
						editableCommentsStore={editableCommentsStore}
						className={className}
						newCommentRows={newCommentRows}
						commentsLocked={commentsLocked}
						onToggleLock={onToggleLock}
					/>
				)}
			</>
		);
	},
);
