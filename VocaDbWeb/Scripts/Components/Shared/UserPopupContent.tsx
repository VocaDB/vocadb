import { ProfileIcon } from '@/Components/Shared/Partials/User/ProfileIcon';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import dayjs from '@/dayjs';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface UserPopupContentProps {
	user: UserApiContract;
}

export const UserPopupContent = React.memo(
	({ user }: UserPopupContentProps): React.ReactElement => {
		const { t } = useTranslation(['Resources']);

		return (
			<>
				{user.mainPicture && user.mainPicture.urlThumb && (
					<div className="thumbnail">
						<ProfileIcon url={user.mainPicture.urlThumb} />
					</div>
				)}

				<strong className="popupTitle">{user.name}</strong>

				<p>{t(`Resources:UserGroupNames.${user.groupId}`)}</p>

				{user.verifiedArtist && (
					<p>{t('ViewRes.User:Details.VerifiedAccount')}</p>
				)}

				<p>
					Joined{/* LOC */} {dayjs(user.memberSince).format('l')}
				</p>
			</>
		);
	},
);
