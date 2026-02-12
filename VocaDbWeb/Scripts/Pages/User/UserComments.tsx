import { EditableComments } from '@/Components/Shared/Partials/Comment/EditableComments';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import { useLoginManager } from '@/LoginManagerContext';
import { UserDetailsNav } from '@/Pages/User/UserDetailsRoutes';
import { UserDetailsStore } from '@/Stores/User/UserDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';

interface UserCommentsProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserComments = observer(
	({ user, userDetailsStore }: UserCommentsProps): React.ReactElement => {
		const loginManager = useLoginManager();

		React.useEffect(() => {
			userDetailsStore.comments.initComments();
		}, [userDetailsStore]);

		return (
			<>
				<UserDetailsNav user={user} tab="comments" />

				<div className="well well-transparent">
					<EditableComments
						editableCommentsStore={userDetailsStore.comments}
						allowCreateComment={
							!user.standalone && loginManager.canCreateComments && (!userDetailsStore.comments.commentsLocked || loginManager.canLockComments)
						}
						well={false}
						comments={userDetailsStore.comments.pageOfComments}
						commentsLocked={userDetailsStore.comments.commentsLocked}
						onToggleLock={userDetailsStore.comments.toggleCommentsLocked}
					/>
				</div>
			</>
		);
	},
);

export default UserComments;
