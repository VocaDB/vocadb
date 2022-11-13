import { EditableComments } from '@/Components/Shared/Partials/Comment/EditableComments';
import { loginManager } from '@/Models/LoginManager';
import { EditableCommentsStore } from '@/Stores/EditableCommentsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface LatestCommentsKnockoutProps {
	editableCommentsStore: EditableCommentsStore;
}

export const LatestCommentsKnockout = observer(
	({
		editableCommentsStore,
	}: LatestCommentsKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);

		return (
			<>
				<h3 className="withMargin">
					{t('ViewRes:EntryDetails.LatestComments')}
				</h3>
				<div>
					<EditableComments
						editableCommentsStore={editableCommentsStore}
						allowCreateComment={loginManager.canCreateComments}
						well={false}
						comments={editableCommentsStore.topComments}
						newCommentRows={3}
						pagination={false}
					/>
					{editableCommentsStore.comments.length === 0 && (
						<p>{t('ViewRes:EntryDetails.NoComments')}</p>
					)}
				</div>
			</>
		);
	},
);
