import { UserIconLink_UserForApiContract } from '@/Components/Shared/Partials/User/UserIconLink_UserForApiContract';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import React from 'react';

interface UserIconLinkOrName_UserForApiContractProps {
	user?: UserApiContract;
	name: string;
	size?: number;
}

export const UserIconLinkOrName_UserForApiContract = ({
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
