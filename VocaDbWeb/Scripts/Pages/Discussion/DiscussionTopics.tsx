import Breadcrumb from '@Bootstrap/Breadcrumb';
import useVocaDbTitle from '@Components/useVocaDbTitle';
import DiscussionIndexStore from '@Stores/Discussion/DiscussionIndexStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import { DiscussionLayout } from './DiscussionRoutes';
import ViewTopic from './Partials/ViewTopic';

interface DiscussionTopicsProps {
	discussionIndexStore: DiscussionIndexStore;
}

const DiscussionTopics = observer(
	({ discussionIndexStore }: DiscussionTopicsProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes.Discussion']);

		const title = t('ViewRes.Discussion:Index.Discussions');

		useVocaDbTitle(title, ready);

		const { topicId } = useParams();

		React.useEffect(() => {
			discussionIndexStore.selectTopicById(Number(topicId));
		}, [discussionIndexStore, topicId]);

		return (
			<DiscussionLayout title={title}>
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
