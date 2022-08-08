import UserDetailsContract from '@DataContracts/User/UserDetailsContract';
import LoginManager from '@Models/LoginManager';
import UserDetailsStore from '@Stores/User/UserDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';

import EditableComments from '../../Components/Shared/Partials/Comment/EditableComments';
import { UserDetailsNav } from './UserDetailsRoutes';

const loginManager = new LoginManager(vdb.values);

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
