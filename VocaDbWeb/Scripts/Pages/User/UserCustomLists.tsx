import SafeAnchor from '@/Bootstrap/SafeAnchor';
import { SongListsKnockout } from '@/Components/Shared/Partials/Song/SongListsKnockout';
import { SongListsFilters } from '@/Components/Shared/Partials/SongListsFilters';
import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import { useLoginManager } from '@/LoginManagerContext';
import { UserDetailsNav } from '@/Pages/User/UserDetailsRoutes';
import {
	UserDetailsStore,
	UserSongListsStore,
} from '@/Stores/User/UserDetailsStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import { observer } from 'mobx-react-lite';
import React from 'react';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';

interface SongListsProps {
	user: UserDetailsContract;
	songLists: UserSongListsStore;
}

const SongLists = observer(
	({ user, songLists }: SongListsProps): React.ReactElement => {
		const loginManager = useLoginManager();

		const { t } = useTranslation(['ViewRes', 'ViewRes.User']);

		useLocationStateStore(songLists);

		const ownProfile =
			loginManager.loggedUser &&
			loginManager.loggedUser.id === user.id &&
			loginManager.loggedUser.active;

		return (
			<>
				<SongListsFilters songListsBaseStore={songLists} />

				<SongListsKnockout songListsBaseStore={songLists} groupByYear={true} />

				{songLists.hasMore && (
					<h3>
						<SafeAnchor href="#" onClick={songLists.loadMore}>
							{t('ViewRes:Shared.ShowMore')}
						</SafeAnchor>
					</h3>
				)}

				{ownProfile && (
					<>
						<Link to="/SongList/Edit" className="textLink addLink">
							{t('ViewRes.User:Details.CreateNewList')}
						</Link>{' '}
						<Link to="/SongList/Import" className="textLink wandIcon">
							{t('ViewRes.User:Details.ImportSongList')}
						</Link>
					</>
				)}
			</>
		);
	},
);

interface UserCustomListsProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserCustomLists = ({
	user,
	userDetailsStore,
}: UserCustomListsProps): React.ReactElement => {
	return (
		<>
			<UserDetailsNav user={user} tab="customLists" />

			<SongLists user={user} songLists={userDetailsStore.songLists} />
		</>
	);
};

export default UserCustomLists;
