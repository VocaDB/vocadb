import Layout from '@Components/Shared/Layout';
import UserGroup from '@Models/Users/UserGroup';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import ListUsersStore from '@Stores/User/ListUsersStore';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ListUsers from './Partials/ListUsers';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);
const listUsersStore = new ListUsersStore(userRepo);

// TODO: use this
interface UserIndexQueryParams {
	filter?: string;
	groupId?: UserGroup;
}

const UserIndex = (): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	return (
		<Layout title={t('ViewRes:Shared.Users')}>
			<ListUsers listUsersStore={listUsersStore} />
		</Layout>
	);
};

export default UserIndex;
