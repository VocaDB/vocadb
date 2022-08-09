import Button from '@/Bootstrap/Button';
import { CommentBodyKnockout } from '@/Components/Shared/Partials/Comment/CommentBodyKnockout';
import { CommentStore } from '@/Stores/CommentStore';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface CommentBodyEditableKnockoutProps {
	editableCommentsStore: EditableCommentsStore;
	commentStore: CommentStore;
	message: string;
}

export const CommentBodyEditableKnockout = observer(
	({
		editableCommentsStore,
		commentStore,
		message,
	}: CommentBodyEditableKnockoutProps): React.ReactElement => {
		const { t } = useTranslation();

		return editableCommentsStore.editCommentStore === commentStore ? (
			<form
				onSubmit={(e): void => {
					e.preventDefault();
					editableCommentsStore.saveEditedComment();
				}}
			>
				<textarea
					value={commentStore.editedMessage}
					onChange={(e): void =>
						runInAction(() => {
							commentStore.editedMessage = e.target.value;
						})
					}
					rows={6}
					cols={60}
					maxLength={3000}
					className="comment-text-edit"
					required
				/>
				<Button type="submit" variant="primary">
					{t('ViewRes:Shared.Save')}
				</Button>{' '}
				<Button onClick={(): void => editableCommentsStore.cancelEditComment()}>
					{t('ViewRes:Shared.Cancel')}
				</Button>
			</form>
		) : (
			<div>
				<CommentBodyKnockout message={message} />
			</div>
		);
	},
);
