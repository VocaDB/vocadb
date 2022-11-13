import Button from '@/Bootstrap/Button';
import { ServerSidePaging } from '@/Components/Shared/Partials/Knockout/ServerSidePaging';
import { ProfileIconKnockout_ImageSize } from '@/Components/Shared/Partials/User/ProfileIconKnockout_ImageSize';
import { DiscussionTopicContract } from '@/DataContracts/Discussion/DiscussionTopicContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import { loginManager } from '@/Models/LoginManager';
import { useMutedUsers } from '@/MutedUsersContext';
import EditTopic from '@/Pages/Discussion/Partials/EditTopic';
import { DiscussionIndexStore } from '@/Stores/Discussion/DiscussionIndexStore';
import classNames from 'classnames';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';

const ViewFolderTableHeader = React.memo(
	(): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Discussion']);

		return (
			<thead>
				<tr>
					<th>{t('ViewRes.Discussion:Index.Topic')}</th>
					<th>{t('ViewRes.Discussion:Index.Author')}</th>
					<th className="hidden-phone">
						{t('ViewRes.Discussion:Index.Comments')}
					</th>
					<th>{t('ViewRes.Discussion:Index.Created')}</th>
					<th>{t('ViewRes.Discussion:Index.LastComment')}</th>
				</tr>
			</thead>
		);
	},
);

interface ViewFolderTableRowProps {
	topic: DiscussionTopicContract;
}

const ViewFolderTableRow = observer(
	({ topic }: ViewFolderTableRowProps): React.ReactElement => {
		const navigate = useNavigate();

		const mutedUsers = useMutedUsers();
		if (mutedUsers.includes(topic.author.id)) return <></>;

		return (
			<tr onClick={(): void => navigate(`/discussion/topics/${topic.id}`)}>
				<td>
					<span>{topic.name}</span>
				</td>
				<td>
					<span>
						{/* eslint-disable-next-line react/jsx-pascal-case */}
						<ProfileIconKnockout_ImageSize
							imageSize={ImageSize.TinyThumb}
							user={topic.author}
							size={18}
						/>{' '}
						<span>{topic.author.name}</span>
					</span>
				</td>
				<td className="hidden-phone">
					<span>{topic.commentCount}</span>
				</td>
				<td>
					<span>{DateTimeHelper.DateTime_format_lll(topic.created)}</span>
				</td>
				<td>
					{topic.lastComment && (
						<span>
							{DateTimeHelper.DateTime_format_lll(topic.lastComment.created)} by{' '}
							{topic.lastComment.authorName}
						</span>
					)}
				</td>
			</tr>
		);
	},
);

interface ViewFolderTableBodyProps {
	discussionIndexStore: DiscussionIndexStore;
}

const ViewFolderTableBody = observer(
	({ discussionIndexStore }: ViewFolderTableBodyProps): React.ReactElement => {
		return (
			<tbody className="discussion-topics">
				{discussionIndexStore.topics.map((topic) => (
					<ViewFolderTableRow topic={topic} key={topic.id} />
				))}
			</tbody>
		);
	},
);

interface ViewFolderTableProps {
	discussionIndexStore: DiscussionIndexStore;
}

const ViewFolderTable = observer(
	({ discussionIndexStore }: ViewFolderTableProps): React.ReactElement => {
		return (
			<table className="table">
				<ViewFolderTableHeader />
				<ViewFolderTableBody discussionIndexStore={discussionIndexStore} />
			</table>
		);
	},
);

interface ViewFolderProps {
	discussionIndexStore: DiscussionIndexStore;
}

const ViewFolder = observer(
	({ discussionIndexStore }: ViewFolderProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Discussion']);
		const navigate = useNavigate();

		return (
			<div /* TODO */ className="discussion-folder">
				<ul className="nav nav-pills folder-list">
					{discussionIndexStore.folders.map((folder) => (
						<li
							className={classNames(
								folder === discussionIndexStore.selectedFolder && 'active',
							)}
							key={folder.id}
						>
							<Link
								to={`/discussion/folders/${folder.id}`}
								title={folder.description}
							>
								{folder.name}
							</Link>
						</li>
					))}
				</ul>

				{discussionIndexStore.selectedFolder?.description && (
					<p className="folder-description">
						{discussionIndexStore.selectedFolder.description}
					</p>
				)}

				{loginManager.canCreateComments && (
					<>
						{discussionIndexStore.showCreateNewTopic ? (
							<form
								onSubmit={(e): void => {
									e.preventDefault();
									discussionIndexStore
										.createNewTopic()
										.then((topic) =>
											navigate(`/discussion/topics/${topic.id}`),
										);
								}}
								className="well well-transparent"
							>
								<EditTopic
									discussionTopicEditStore={discussionIndexStore.newTopic}
								/>
								<Button type="submit" variant="primary">
									{t('ViewRes.Discussion:Index.DoPost')}
								</Button>
							</form>
						) : (
							<Button
								onClick={(): void =>
									runInAction(() => {
										discussionIndexStore.showCreateNewTopic = true;
									})
								}
								className="create-topic"
							>
								<i className="icon-comment"></i>{' '}
								{t('ViewRes.Discussion:Index.WriteNewPost')}
							</Button>
						)}
					</>
				)}

				<ServerSidePaging pagingStore={discussionIndexStore.paging} />

				<ViewFolderTable discussionIndexStore={discussionIndexStore} />

				<ServerSidePaging pagingStore={discussionIndexStore.paging} />
			</div>
		);
	},
);

export default ViewFolder;
