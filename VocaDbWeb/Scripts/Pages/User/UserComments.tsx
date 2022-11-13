import { EditableComments } from '@/Components/Shared/Partials/Comment/EditableComments';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import { loginManager } from '@/Models/LoginManager';
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
							!user.standalone && loginManager.canCreateComments
						}
						well={false}
						comments={userDetailsStore.comments.pageOfComments}
					/>
				</div>
			</>
		);
	},
);

export default UserComments;
