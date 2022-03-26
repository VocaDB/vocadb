import UserApiContract from '@DataContracts/User/UserApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { Link } from 'react-router-dom';

import ProfileIconKnockout from './ProfileIconKnockout';

interface IconAndNameLinkKnockoutProps {
	user: UserApiContract;
	iconSize?: number;
	className?: string;
}

const IconAndNameLinkKnockout = React.memo(
	({
		user,
		iconSize = 20,
		className,
	}: IconAndNameLinkKnockoutProps): React.ReactElement => {
		return (
			<Link
				className={className}
				to={EntryUrlMapper.details_user_byName(user.name)}
			>
				<ProfileIconKnockout
					icon={user.mainPicture?.urlThumb}
					size={iconSize}
				/>{' '}
				<span>{user.name}</span>
			</Link>
		);
	},
);

export default IconAndNameLinkKnockout;
