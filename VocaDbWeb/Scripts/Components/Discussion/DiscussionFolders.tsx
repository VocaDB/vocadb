import Breadcrumb from '@Bootstrap/Breadcrumb';
import useStoreWithPaging from '@Components/useStoreWithPaging';
import DiscussionIndexStore from '@Stores/Discussion/DiscussionIndexStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, useParams } from 'react-router-dom';

import { DiscussionLayout } from './DiscussionRoutes';
import ViewFolder from './Partials/ViewFolder';

const useDiscussionIndexStore = (
	discussionIndexStore: DiscussionIndexStore,
): void => {
	const { folderId } = useParams();

	React.useEffect(() => {
		discussionIndexStore.selectFolderById(Number(folderId));
	}, [discussionIndexStore, discussionIndexStore.folders, folderId]);

	useStoreWithPaging(discussionIndexStore);

	React.useEffect(() => {
		discussionIndexStore.updateResults(true);
	}, [discussionIndexStore, discussionIndexStore.folders, folderId]);
};

interface DiscussionFoldersProps {
	discussionIndexStore: DiscussionIndexStore;
}

const DiscussionFolders = observer(
	({ discussionIndexStore }: DiscussionFoldersProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.Discussion']);

		useDiscussionIndexStore(discussionIndexStore);

		return (
			<DiscussionLayout>
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
