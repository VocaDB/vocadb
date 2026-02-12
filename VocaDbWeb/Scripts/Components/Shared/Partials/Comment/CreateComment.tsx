import Button from '@/Bootstrap/Button';
import JQueryUIButton from '@/JQueryUI/JQueryUIButton';
import { useLoginManager } from '@/LoginManagerContext';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface CreateCommentProps {
	editableCommentsStore: EditableCommentsStore;
	className: string;
	newCommentRows: number;
	commentsLocked: boolean;
	onToggleLock?: () => void;
}

export const CreateComment = observer(
	({
		editableCommentsStore,
		className,
		newCommentRows,
		commentsLocked = false,
		onToggleLock,
	}: CreateCommentProps): React.ReactElement => {
		const loginManager = useLoginManager();
		const { t } = useTranslation(['ViewRes']);

		return (
			<div className={classNames('create-comment', className)}>
				<form
					onSubmit={(e): void => {
						e.preventDefault();
						editableCommentsStore.createComment();
					}}
				>
					<textarea
						value={editableCommentsStore.newComment}
						onChange={(e): void =>
							runInAction(() => {
								editableCommentsStore.newComment = e.target.value;
							})
						}
						rows={newCommentRows}
						cols={60}
						maxLength={2000}
						className="comment-text-edit"
						placeholder={t('ViewRes:DiscussionContent.NewComment')}
						required
					/>
					<Button type="submit" variant="primary">
						{t('ViewRes:DiscussionContent.AddComment')}
					</Button>

					{loginManager.canLockComments && onToggleLock && (
						<JQueryUIButton
							as="button"
							onClick={onToggleLock}
							style={{ marginBottom: 10 }}
							icons={{
								primary: commentsLocked ? 'ui-icon-locked' : 'ui-icon-unlocked',
							}}
						>
							{commentsLocked
								? t('ViewRes:DiscussionContent.UnlockComments')
								: t('ViewRes:DiscussionContent.LockComments')}
						</JQueryUIButton>
					)}
				</form>
			</div>
		);
	},
);
