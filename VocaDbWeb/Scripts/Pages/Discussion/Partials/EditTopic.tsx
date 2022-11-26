import { Markdown } from '@/Components/KnockoutExtensions/Markdown';
import { useLoginManager } from '@/LoginManagerContext';
import { DiscussionTopicEditStore } from '@/Stores/Discussion/DiscussionTopicEditStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface EditTopicProps {
	discussionTopicEditStore: DiscussionTopicEditStore;
}

const EditTopic = observer(
	({ discussionTopicEditStore }: EditTopicProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation(['ViewRes.Discussion']);

		return (
			<div>
				<label>{t('ViewRes.Discussion:Index.Topic')}</label>
				<input
					value={discussionTopicEditStore.name}
					onChange={(e): void =>
						runInAction(() => {
							discussionTopicEditStore.name = e.target.value;
						})
					}
					type="text"
					className="input-xlarge"
					maxLength={200}
					required
				/>

				<div className="discussion-topic-edit-content">
					<div className="edit-text">
						<label>{t('ViewRes.Discussion:Index.PostContent')}</label>
						<textarea
							value={discussionTopicEditStore.content}
							onChange={(e): void =>
								runInAction(() => {
									discussionTopicEditStore.content = e.target.value;
								})
							}
							cols={60}
							rows={6}
							required
						/>
					</div>

					<div className="edit-preview">
						<label>{t('ViewRes.Discussion:Index.Preview')}</label>
						<div>
							<Markdown>{discussionTopicEditStore.content}</Markdown>
						</div>
					</div>

					{loginManager.canDeleteComments && (
						<>
							{discussionTopicEditStore.folders.length > 0 && (
								<div>
									<label>Folder{/* LOC */}</label>
									<select
										value={discussionTopicEditStore.folderId}
										onChange={(e): void =>
											runInAction(() => {
												discussionTopicEditStore.folderId = Number(
													e.target.value,
												);
											})
										}
									>
										{discussionTopicEditStore.folders.map((folder) => (
											<option value={folder.id} key={folder.id}>
												{folder.name}
											</option>
										))}
									</select>
								</div>
							)}
							<p>
								<label>
									<input
										type="checkbox"
										checked={discussionTopicEditStore.locked}
										onChange={(e): void =>
											runInAction(() => {
												discussionTopicEditStore.locked = e.target.checked;
											})
										}
									/>{' '}
									Locked{/* LOC */}
								</label>
							</p>
						</>
					)}
				</div>
			</div>
		);
	},
);

export default EditTopic;
