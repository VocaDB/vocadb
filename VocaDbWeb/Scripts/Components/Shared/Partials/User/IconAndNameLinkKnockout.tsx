import SafeAnchor from '@Bootstrap/SafeAnchor';
import UserApiContract from '@DataContracts/User/UserApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

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
			<SafeAnchor
				className={className}
				href={EntryUrlMapper.details_user_byName(user.name)}
			>
				<ProfileIconKnockout
					icon={user.mainPicture?.urlThumb}
					size={iconSize}
				/>{' '}
				<span>{user.name}</span>
			</SafeAnchor>
		);
	},
);

export default IconAndNameLinkKnockout;
