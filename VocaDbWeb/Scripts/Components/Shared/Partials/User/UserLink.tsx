import { UserBaseContract } from '@/DataContracts/User/UserBaseContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface UserLinkProps {
	user: UserBaseContract;
}

export const UserLink = React.memo(
	({ user }: UserLinkProps): React.ReactElement => {
		return (
			<Link to={EntryUrlMapper.details_user_byName(user.name)}>
				{user.name}
			</Link>
		);
	},
);
