import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { DiscussionLayout } from '@/Pages/Discussion/DiscussionRoutes';
import ViewTopic from '@/Pages/Discussion/Partials/ViewTopic';
import { DiscussionIndexStore } from '@/Stores/Discussion/DiscussionIndexStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface DiscussionTopicsProps {
	discussionIndexStore: DiscussionIndexStore;
}

const DiscussionTopics = observer(
	({ discussionIndexStore }: DiscussionTopicsProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes.Discussion']);

		const title = t('ViewRes.Discussion:Index.Discussions');

		const { topicId } = useParams();

		React.useEffect(() => {
			discussionIndexStore.selectTopicById(Number(topicId));
		}, [discussionIndexStore, topicId]);

		return (
			<DiscussionLayout pageTitle={title} ready={ready} title={title}>
				<Breadcrumb>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{ to: '/discussion' }}
						divider
					>
						{t('ViewRes.Discussion:Index.Discussions')}
					</Breadcrumb.Item>
					<Breadcrumb.Item
						linkAs={Link}
						linkProps={{
							to: `/discussion/folders/${discussionIndexStore.selectedFolder?.id}`,
						}}
					>
						{discussionIndexStore.selectedFolder?.name}
					</Breadcrumb.Item>
				</Breadcrumb>

				{discussionIndexStore.selectedTopic && (
					<ViewTopic
						discussionIndexStore={discussionIndexStore}
						discussionTopicStore={discussionIndexStore.selectedTopic}
					/>
				)}
			</DiscussionLayout>
		);
	},
);

export default DiscussionTopics;
