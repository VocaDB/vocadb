import Alert from '@/Bootstrap/Alert';
import Button from '@/Bootstrap/Button';
import { CommentKnockout } from '@/Components/Shared/Partials/Comment/CommentKnockout';
import { EditableComments } from '@/Components/Shared/Partials/Comment/EditableComments';
import { loginManager } from '@/Models/LoginManager';
import EditTopic from '@/Pages/Discussion/Partials/EditTopic';
import { DiscussionIndexStore } from '@/Stores/Discussion/DiscussionIndexStore';
import { DiscussionTopicStore } from '@/Stores/Discussion/DiscussionTopicStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';

interface ViewTopicProps {
	discussionIndexStore: DiscussionIndexStore;
	discussionTopicStore: DiscussionTopicStore;
}

const ViewTopic = observer(
	({
		discussionIndexStore,
		discussionTopicStore,
	}: ViewTopicProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes']);
		const navigate = useNavigate();

		useLocationStateStore(discussionTopicStore);

		return (
			<div>
				{discussionTopicStore.isBeingEdited ? (
					<form
						onSubmit={(e): void => {
							e.preventDefault();
							discussionTopicStore.saveEditedTopic();
						}}
						className="well well-transparent"
					>
						{discussionTopicStore.editStore && (
							<EditTopic
								discussionTopicEditStore={discussionTopicStore.editStore}
							/>
						)}
						<Button type="submit" variant="primary">
							{t('ViewRes:Shared.Save')}
						</Button>{' '}
						<Button onClick={(): void => discussionTopicStore.cancelEdit()}>
							{t('ViewRes:Shared.Cancel')}
						</Button>
					</form>
				) : (
					<div>
						<div className="well well-transparent">
							<h3>{discussionTopicStore.contract.name}</h3>

							<div>
								<CommentKnockout
									commentKnockoutStore={discussionTopicStore.contract}
									message={discussionTopicStore.contract.content}
									allowMarkdown={true}
									onDelete={(): void => {
										discussionIndexStore
											.deleteTopic(discussionTopicStore.contract)
											.then(() =>
												navigate(
													`/discussion/folders/${discussionTopicStore.contract.folderId}`,
												),
											);
									}}
									onEdit={(): void => discussionTopicStore.beginEditTopic()}
								/>
							</div>
						</div>
					</div>
				)}

				{discussionTopicStore.contract.locked ? (
					<Alert>Comments have been disabled for this topic.{/* LOC */}</Alert>
				) : (
					<div>
						<EditableComments
							editableCommentsStore={discussionTopicStore.comments}
							allowCreateComment={loginManager.canCreateComments}
							comments={discussionTopicStore.comments.pageOfComments}
							commentBoxEnd={true}
						/>
					</div>
				)}
			</div>
		);
	},
);

export default ViewTopic;
