import ArtistDetailsContract from '@/DataContracts/Artist/ArtistDetailsContract';
import LoginManager from '@/Models/LoginManager';
import ArtistDetailsStore from '@/Stores/Artist/ArtistDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';

import EditableComments from '../../Components/Shared/Partials/Comment/EditableComments';
import { ArtistDetailsTabs } from './ArtistDetailsRoutes';

const loginManager = new LoginManager(vdb.values);

interface ArtistDiscussionProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistDiscussion = observer(
	({
		artist,
		artistDetailsStore,
	}: ArtistDiscussionProps): React.ReactElement => {
		React.useEffect(() => {
			artistDetailsStore.comments.initComments();
		}, [artistDetailsStore]);

		return (
			<ArtistDetailsTabs
				artist={artist}
				artistDetailsStore={artistDetailsStore}
				tab="discussion"
			>
				<EditableComments
					editableCommentsStore={artistDetailsStore.comments}
					allowCreateComment={loginManager.canCreateComments}
					well={false}
					comments={artistDetailsStore.comments.pageOfComments}
				/>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistDiscussion;
