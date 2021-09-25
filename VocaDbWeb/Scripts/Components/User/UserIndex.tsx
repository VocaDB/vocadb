import Layout from '@Components/Shared/Layout';
import useRouteParamsTracking from '@Components/useRouteParamsTracking';
import useStoreWithPaging from '@Components/useStoreWithPaging';
import useVocaDbTitle from '@Components/useVocaDbTitle';
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

const UserIndex = (): React.ReactElement => {
	const { t, ready } = useTranslation(['ViewRes']);

	const title = t('ViewRes:Shared.Users');

	useVocaDbTitle(title, ready);

	useStoreWithPaging(listUsersStore);
	useRouteParamsTracking(listUsersStore, ready);

	return (
		<Layout title={title}>
			<ListUsers listUsersStore={listUsersStore} />
		</Layout>
	);
};

export default UserIndex;
