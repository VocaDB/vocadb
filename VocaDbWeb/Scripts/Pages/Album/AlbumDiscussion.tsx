import AlbumDetailsForApi from '@/DataContracts/Album/AlbumDetailsForApi';
import LoginManager from '@/Models/LoginManager';
import AlbumDetailsStore from '@/Stores/Album/AlbumDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';

import EditableComments from '../../Components/Shared/Partials/Comment/EditableComments';
import { AlbumDetailsTabs } from './AlbumDetailsRoutes';

const loginManager = new LoginManager(vdb.values);

interface AlbumDiscussionProps {
	model: AlbumDetailsForApi;
	albumDetailsStore: AlbumDetailsStore;
}

const AlbumDiscussion = observer(
	({ model, albumDetailsStore }: AlbumDiscussionProps): React.ReactElement => {
		React.useEffect(() => {
			albumDetailsStore.comments.initComments();
		}, [albumDetailsStore]);

		return (
			<AlbumDetailsTabs
				model={model}
				albumDetailsStore={albumDetailsStore}
				tab="discussion"
			>
				<EditableComments
					editableCommentsStore={albumDetailsStore.comments}
					allowCreateComment={loginManager.canCreateComments}
					well={false}
					comments={albumDetailsStore.comments.pageOfComments}
				/>
			</AlbumDetailsTabs>
		);
	},
);

export default AlbumDiscussion;
