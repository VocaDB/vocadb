import { Layout } from '@/Components/Shared/Layout';
import { useVdbTitle } from '@/Components/useVdbTitle';
import ListUsers from '@/Pages/User/Partials/ListUsers';
import { userRepo } from '@/Repositories/UserRepository';
import { ListUsersStore } from '@/Stores/User/ListUsersStore';
import { useLocationStateStore } from '@vocadb/route-sphere';
import React from 'react';
import { useTranslation } from 'react-i18next';

const listUsersStore = new ListUsersStore(userRepo);

const UserIndex = (): React.ReactElement => {
	const { t, ready } = useTranslation(['ViewRes']);

	const title = t('ViewRes:Shared.Users');

	useVdbTitle(title, ready);

	useLocationStateStore(listUsersStore);

	return (
		<Layout title={title}>
			<ListUsers listUsersStore={listUsersStore} />
		</Layout>
	);
};

export default UserIndex;
