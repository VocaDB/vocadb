import { UserLink } from '@/Components/Shared/Partials/User/UserLink';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import React from 'react';

interface UserLinkOrNameProps {
	user?: UserApiContract;
	name: string;
}

export const UserLinkOrName = React.memo(
	({ user, name }: UserLinkOrNameProps): React.ReactElement => {
		return user ? <UserLink user={user} /> : <>{name}</>;
	},
);
