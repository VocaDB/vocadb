import { EditableComments } from '@/Components/Shared/Partials/Comment/EditableComments';
import { ArtistDetailsContract } from '@/DataContracts/Artist/ArtistDetailsContract';
import { useLoginManager } from '@/LoginManagerContext';
import { ArtistDetailsTabs } from '@/Pages/Artist/ArtistDetailsRoutes';
import { ArtistDetailsStore } from '@/Stores/Artist/ArtistDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';

interface ArtistDiscussionProps {
	artist: ArtistDetailsContract;
	artistDetailsStore: ArtistDetailsStore;
}

const ArtistDiscussion = observer(
	({
		artist,
		artistDetailsStore,
	}: ArtistDiscussionProps): React.ReactElement => {
		const loginManager = useLoginManager();

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
					allowCreateComment={loginManager.canCreateComments && (!artistDetailsStore.comments.commentsLocked || loginManager.canLockComments)}
					well={false}
					comments={artistDetailsStore.comments.pageOfComments}
					commentsLocked={artistDetailsStore.comments.commentsLocked}
					onToggleLock={artistDetailsStore.comments.toggleCommentsLocked}
				/>
			</ArtistDetailsTabs>
		);
	},
);

export default ArtistDiscussion;
