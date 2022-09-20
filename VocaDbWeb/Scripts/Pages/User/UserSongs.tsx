import { UserDetailsContract } from '@/DataContracts/User/UserDetailsContract';
import RatedSongs from '@/Pages/User/Partials/RatedSongs';
import { UserDetailsNav } from '@/Pages/User/UserDetailsRoutes';
import { UserDetailsStore } from '@/Stores/User/UserDetailsStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import React from 'react';

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

	useLocationStateStore(userDetailsStore.ratedSongsStore);

	return (
		<>
			<UserDetailsNav user={user} tab="songs" />

			<RatedSongs ratedSongsStore={userDetailsStore.ratedSongsStore} />
		</>
	);
};

export default UserSongs;
