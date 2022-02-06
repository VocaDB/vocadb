import UserApiContract from '@DataContracts/User/UserApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

interface NameLinkKnockoutProps {
	user: UserApiContract;
}

const NameLinkKnockout = ({
	user,
}: NameLinkKnockoutProps): React.ReactElement => {
	return (
		// User name inside anchor
		<a href={EntryUrlMapper.details_user_byName(user.name)}>
			<span>{user.name}</span>
		</a>
	);
};

export default NameLinkKnockout;
