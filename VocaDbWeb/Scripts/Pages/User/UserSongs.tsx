import UserDetailsContract from '@/DataContracts/User/UserDetailsContract';
import UserDetailsStore from '@/Stores/User/UserDetailsStore';
import { useStoreWithPagination } from '@vocadb/route-sphere';
import React from 'react';

import RatedSongs from './Partials/RatedSongs';
import { UserDetailsNav } from './UserDetailsRoutes';

interface UserSongsProps {
	user: UserDetailsContract;
	userDetailsStore: UserDetailsStore;
}

const UserSongs = ({
	user,
	userDetailsStore,
}: UserSongsProps): React.ReactElement => {
	React.useEffect(() => {
		userDetailsStore.ratedSongsStore.init();
	}, [userDetailsStore]);

	useStoreWithPagination(userDetailsStore.ratedSongsStore);

	return (
		<>
			<UserDetailsNav user={user} tab="songs" />

			<RatedSongs ratedSongsStore={userDetailsStore.ratedSongsStore} />
		</>
	);
};

export default UserSongs;
