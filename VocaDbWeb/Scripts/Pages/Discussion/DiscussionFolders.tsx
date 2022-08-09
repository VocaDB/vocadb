import Breadcrumb from '@/Bootstrap/Breadcrumb';
import { useVocaDbTitle } from '@/Components/useVocaDbTitle';
import { DiscussionLayout } from '@/Pages/Discussion/DiscussionRoutes';
import ViewFolder from '@/Pages/Discussion/Partials/ViewFolder';
import { DiscussionIndexStore } from '@/Stores/Discussion/DiscussionIndexStore';
import { useStoreWithPagination } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

interface DiscussionFoldersProps {
	discussionIndexStore: DiscussionIndexStore;
}

const DiscussionFolders = observer(
	({ discussionIndexStore }: DiscussionFoldersProps): React.ReactElement => {
		const { t, ready } = useTranslation(['ViewRes.Discussion']);

		const title = t('ViewRes.Discussion:Index.Discussions');

		useVocaDbTitle(title, ready);

		const { folderId } = useParams();

		React.useEffect(() => {
			discussionIndexStore.selectFolderById(Number(folderId));
		}, [discussionIndexStore, discussionIndexStore.folders, folderId]);

		useStoreWithPagination(discussionIndexStore);

		React.useEffect(() => {
			discussionIndexStore.updateResults(true);
		}, [discussionIndexStore, discussionIndexStore.folders, folderId]);

		return (
			<DiscussionLayout title={title}>
				<Breadcrumb>
					<Breadcrumb.Item linkAs={Link} linkProps={{ to: '/discussion' }}>
						{t('ViewRes.Discussion:Index.Discussions')}
					</Breadcrumb.Item>
				</Breadcrumb>

				<ViewFolder discussionIndexStore={discussionIndexStore} />
			</DiscussionLayout>
		);
	},
);

export default DiscussionFolders;
