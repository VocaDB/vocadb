import SafeAnchor from '@Bootstrap/SafeAnchor';
import UserApiContract from '@DataContracts/User/UserApiContract';
import ImageSize from '@Models/Images/ImageSize';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';

import ProfileIconKnockout_ImageSize from './ProfileIconKnockout_ImageSize';

interface IconAndLinkKnockoutProps {
	user: UserApiContract;
}

// User icon inside anchor (no name)
const IconAndLinkKnockout = React.memo(
	({ user }: IconAndLinkKnockoutProps): React.ReactElement => {
		return (
			<SafeAnchor
				href={EntryUrlMapper.details_user_byName(user.name)}
				className="pull-left"
			>
				<ProfileIconKnockout_ImageSize
					imageSize={ImageSize.Thumb}
					user={user}
					size={70}
				/>
			</SafeAnchor>
		);
	},
);

export default IconAndLinkKnockout;
