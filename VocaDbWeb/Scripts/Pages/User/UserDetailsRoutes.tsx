import UserDetailsContract from '@/DataContracts/User/UserDetailsContract';
import EntryUrlMapper from '@/Shared/EntryUrlMapper';
import UserDetailsStore from '@/Stores/User/UserDetailsStore';
import classNames from 'classnames';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link, Route, Routes } from 'react-router-dom';

import UserAlbums from './UserAlbums';
import UserArtists from './UserArtists';
import UserComments from './UserComments';
import UserCustomLists from './UserCustomLists';
import UserEvents from './UserEvents';
import UserOverview from './UserOverview';
import UserSongs from './UserSongs';

interface UserDetailsNavProps {
	user: UserDetailsContract;
	tab:
		| 'overview'
		| 'artists'
		| 'albums'
		| 'songs'
		| 'customLists'
		| 'comments'
		| 'events';
}

export const UserDetailsNav = React.memo(
	({ user, tab }: UserDetailsNavProps): React.ReactElement => {
		const { t } = useTranslation(['ViewRes.User']);

		return (
			<ul className="nav nav-pills">
				<li className={classNames(tab === 'overview' && 'active')}>
					<Link to={`${EntryUrlMapper.details_user_byName(user.name)}`}>
						{t('ViewRes.User:Details.Overview')}
					</Link>
				</li>
				<li className={classNames(tab === 'artists' && 'active')}>
					<Link to={`${EntryUrlMapper.details_user_byName(user.name)}/artists`}>
						{t('ViewRes.User:Details.FollowedArtistsTab')}
					</Link>
				</li>
				<li className={classNames(tab === 'albums' && 'active')}>
					<Link to={`${EntryUrlMapper.details_user_byName(user.name)}/albums`}>
						{t('ViewRes.User:Details.CollectionTab')}
					</Link>
				</li>
				<li className={classNames(tab === 'songs' && 'active')}>
					<Link to={`${EntryUrlMapper.details_user_byName(user.name)}/songs`}>
						{t('ViewRes.User:Details.FavoriteSongsTab')}
					</Link>
				</li>
				<li className={classNames(tab === 'customLists' && 'active')}>
					<Link to={`${EntryUrlMapper.details_user_byName(user.name)}/lists`}>
						{t('ViewRes.User:Details.CustomListsTab')}
					</Link>
				</li>
				<li className={classNames(tab === 'comments' && 'active')}>
					<Link
						to={`${EntryUrlMapper.details_user_byName(user.name)}/comments`}
					>
						{t('ViewRes.User:Details.Comments')}
					</Link>
				</li>
				<li className={classNames(tab === 'events' && 'active')}>
					<Link to={`${EntryUrlMapper.details_user_byName(user.name)}/events`}>
						{t('ViewRes.User:Details.Events')}
					</Link>
				</li>
			</ul>
		);
	},
);

interface UserDetailsRoutesProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserDetailsRoutes = ({
	user,
	userDetailsStore,
}: UserDetailsRoutesProps): React.ReactElement => {
	return (
		<Routes>
			<Route
				path="artists"
				element={
					<UserArtists user={user} userDetailsStore={userDetailsStore} />
				}
			/>
			<Route
				path="albums"
				element={<UserAlbums user={user} userDetailsStore={userDetailsStore} />}
			/>
			<Route
				path="songs"
				element={<UserSongs user={user} userDetailsStore={userDetailsStore} />}
			/>
			<Route
				path="lists"
				element={
					<UserCustomLists user={user} userDetailsStore={userDetailsStore} />
				}
			/>
			<Route
				path="comments"
				element={
					<UserComments user={user} userDetailsStore={userDetailsStore} />
				}
			/>
			<Route
				path="events"
				element={<UserEvents user={user} userDetailsStore={userDetailsStore} />}
			/>
			<Route
				path="*"
				element={
					<UserOverview user={user} userDetailsStore={userDetailsStore} />
				}
			/>
		</Routes>
	);
};

export default UserDetailsRoutes;
