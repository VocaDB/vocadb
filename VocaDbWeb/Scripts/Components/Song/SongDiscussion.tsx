import SongDetailsForApi from '@DataContracts/Song/SongDetailsForApi';
import LoginManager from '@Models/LoginManager';
import SongDetailsStore from '@Stores/Song/SongDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';

import EditableComments from '../Shared/Partials/Comment/EditableComments';
import { SongDetailsTabs } from './SongDetailsRoutes';

const loginManager = new LoginManager(vdb.values);

interface SongDiscussionProps {
	model: SongDetailsForApi;
	songDetailsStore: SongDetailsStore;
}

const SongDiscussion = observer(
	({ model, songDetailsStore }: SongDiscussionProps): React.ReactElement => {
		React.useEffect(() => {
			songDetailsStore.comments.initComments();
		}, [songDetailsStore]);

		return (
			<SongDetailsTabs
				model={model}
				songDetailsStore={songDetailsStore}
				tab="discussion"
			>
				<EditableComments
					editableCommentsStore={songDetailsStore.comments}
					allowCreateComment={loginManager.canCreateComments}
					well={false}
					comments={songDetailsStore.comments.pageOfComments}
				/>
			</SongDetailsTabs>
		);
	},
);

export default SongDiscussion;
