import DiscussionIndexStore from '@Stores/Discussion/DiscussionIndexStore';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

import { DiscussionLayout } from './DiscussionRoutes';
import ViewFolders from './Partials/ViewFolders';

interface DiscussionIndexProps {
	discussionIndexStore: DiscussionIndexStore;
}

const DiscussionIndex = observer(
	({ discussionIndexStore }: DiscussionIndexProps): React.ReactElement => {
		React.useEffect(() => {
			runInAction(() => {
				discussionIndexStore.selectedFolder = undefined;
				discussionIndexStore.selectedTopic = undefined;
			});
		}, [discussionIndexStore]);

		return (
			<DiscussionLayout>
				<ViewFolders discussionIndexStore={discussionIndexStore} />
			</DiscussionLayout>
		);
	},
);

export default DiscussionIndex;
