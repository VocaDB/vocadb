import SafeAnchor from '@Bootstrap/SafeAnchor';
import UserApiContract from '@DataContracts/User/UserApiContract';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ProfileIconKnockout from './ProfileIconKnockout';

interface IconNameAndTypeLinkKnockoutProps {
	user: UserApiContract;
	iconSize?: number;
}

const IconNameAndTypeLinkKnockout = React.memo(
	({
		user,
		iconSize = 20,
	}: IconNameAndTypeLinkKnockoutProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<SafeAnchor
				href={EntryUrlMapper.details_user_byName(user.name)}
				title={`${t(`Resources:UserGroupNames.${user.groupId}`)}\nJoined: ${
					user.memberSince
				}`}
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

export default IconNameAndTypeLinkKnockout;
