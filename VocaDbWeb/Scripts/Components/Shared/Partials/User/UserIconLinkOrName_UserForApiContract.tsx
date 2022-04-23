import UserApiContract from '@DataContracts/User/UserApiContract';
import React from 'react';

import UserIconLink_UserForApiContract from './UserIconLink_UserForApiContract';

interface UserIconLinkOrName_UserForApiContractProps {
	user?: UserApiContract;
	name: string;
	size?: number;
}

const UserIconLinkOrName_UserForApiContract = ({
	user,
	name,
	size,
}: UserIconLinkOrName_UserForApiContractProps): React.ReactElement => {
	return user ? (
		// eslint-disable-next-line react/jsx-pascal-case
		<UserIconLink_UserForApiContract user={user} size={size} />
	) : (
		<>{name}</>
	);
};

export default UserIconLinkOrName_UserForApiContract;
