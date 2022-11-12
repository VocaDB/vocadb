import { IconAndNameLinkKnockout } from '@/Components/Shared/Partials/User/IconAndNameLinkKnockout';
import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import { useMutedUsers } from '@/MutedUsersContext';
import { RatingsStore } from '@/Stores/Song/SongDetailsStore';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';

interface UserWithSongRatingProps {
	user: UserApiContract;
}

const UserWithSongRating = observer(
	({ user }: UserWithSongRatingProps): React.ReactElement => {
		const mutedUsers = useMutedUsers();
		if (mutedUsers.includes(user.id)) return <></>;

		return (
			<li className="link-item user-with-rating">
				<IconAndNameLinkKnockout user={user} />
			</li>
		);
	},
);

interface UsersWithSongRatingProps {
	ratingsStore: RatingsStore;
}

const UsersWithSongRating = observer(
	({ ratingsStore }: UsersWithSongRatingProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes', 'ViewRes.Song']);

		return (
			<>
				{ratingsStore.showFavorites && (
					<div>
						<h4>
							{t('ViewRes.Song:Details.FavoritedByTitle')}{' '}
							<span>
								{t('ViewRes:EntryDetails.NumTotalParenthesis', {
									0: ratingsStore.favoritesCount,
								})}
							</span>
						</h4>
						<ul className="link-list">
							{ratingsStore.favorites.map((user, index) => (
								<React.Fragment key={user.id}>
									{index > 0 && ' '}
									<UserWithSongRating user={user} />
								</React.Fragment>
							))}
						</ul>
					</div>
				)}

				{ratingsStore.showLikes && (
					<div className="withMargin">
						<h4>
							{t('ViewRes.Song:Details.LikedByTitle')}{' '}
							<span>
								{t('ViewRes:EntryDetails.NumTotalParenthesis', {
									0: ratingsStore.likesCount,
								})}
							</span>
						</h4>
						<ul className="link-list">
							{ratingsStore.likes.map((user, index) => (
								<React.Fragment key={user.id}>
									{index > 0 && ' '}
									<UserWithSongRating user={user} />
								</React.Fragment>
							))}
						</ul>
					</div>
				)}

				{ratingsStore.hiddenRatingsCount > 0 && (
					<h4 className="withMargin">{
						`${ratingsStore.hiddenRatingsCount} hidden ratings` /* LOC */
					}</h4>
				)}
			</>
		);
	},
);

export default UsersWithSongRating;
