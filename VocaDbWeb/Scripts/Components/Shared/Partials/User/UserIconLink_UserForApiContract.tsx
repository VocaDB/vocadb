import { ProfileIcon } from '@/Components/Shared/Partials/User/ProfileIcon';
import { UserLink } from '@/Components/Shared/Partials/User/UserLink';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { UrlHelper } from '@/Helpers/UrlHelper';
import { ImageSize } from '@/Models/Images/ImageSize';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface UserIconLink_UserForApiContractProps {
	user: UserApiContract;
	size?: number;
	userInfo?: boolean;
	tooltip?: boolean;
}

export const UserIconLink_UserForApiContract = ({
	user,
	size = 20,
	userInfo = false,
	tooltip = false,
}: UserIconLink_UserForApiContractProps): React.ReactElement => {
	const { t } = useTranslation(['Resources']);

	return tooltip ? (
		<UserLink
			user={user}
			tooltip
			title={
				userInfo
					? `${t(`Resources:UserGroupNames.${user.groupId}`)}\nJoined: ${
							user.memberSince
					  }` /* LOC */
					: undefined
			}
		>
			<ProfileIcon
				url={
					user.mainPicture
						? UrlHelper.getSmallestThumb(user.mainPicture, ImageSize.TinyThumb)
						: undefined
				}
				size={size}
			/>{' '}
			<span>{user.name}</span>
		</UserLink>
	) : (
		<Link
			to={EntryUrlMapper.details_user_byName(user.name)}
			title={
				userInfo
					? `${t(`Resources:UserGroupNames.${user.groupId}`)}\nJoined: ${
							user.memberSince
					  }` /* LOC */
					: undefined
			}
		>
			<ProfileIcon
				url={
					user.mainPicture
						? UrlHelper.getSmallestThumb(user.mainPicture, ImageSize.TinyThumb)
						: undefined
				}
				size={size}
			/>{' '}
			<span>{user.name}</span>
		</Link>
	);
};
