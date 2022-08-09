import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { DiscussionLayout } from '@/Pages/Discussion/DiscussionRoutes';
import ViewFolders from '@/Pages/Discussion/Partials/ViewFolders';
import { DiscussionIndexStore } from '@/Stores/Discussion/DiscussionIndexStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface DiscussionIndexProps {
	discussionIndexStore: DiscussionIndexStore;
}

const DiscussionIndex = observer(
	({ discussionIndexStore }: DiscussionIndexProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes.Discussion']);

		const title = t('ViewRes.Discussion:Index.Discussions');

		useVocaDbTitle(title, ready);

		React.useEffect(() => {
			runInAction(() => {
				discussionIndexStore.selectedFolder = undefined;
				discussionIndexStore.selectedTopic = undefined;
			});
		}, [discussionIndexStore]);

		return (
			<DiscussionLayout title={title}>
				<ViewFolders discussionIndexStore={discussionIndexStore} />
			</DiscussionLayout>
		);
	},
);

export default DiscussionIndex;
