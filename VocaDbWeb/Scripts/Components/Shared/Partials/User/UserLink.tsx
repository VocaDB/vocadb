import UserBaseContract from '@DataContracts/User/UserBaseContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface UserLinkProps {
	user: UserBaseContract;
}

const UserLink = React.memo(
	({ user }: UserLinkProps): React.ReactElement => {
		return (
			<a href={EntryUrlMapper.details_user_byName(user.name)}>{user.name}</a>
		);
	},
);

export default UserLink;
