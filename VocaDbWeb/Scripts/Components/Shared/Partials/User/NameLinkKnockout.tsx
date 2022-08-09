import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

interface NameLinkKnockoutProps {
	user: UserApiContract;
}

export const NameLinkKnockout = ({
	user,
}: NameLinkKnockoutProps): React.ReactElement => {
	return (
		// User name inside anchor
		<Link to={EntryUrlMapper.details_user_byName(user.name)}>
			<span>{user.name}</span>
		</Link>
	);
};
