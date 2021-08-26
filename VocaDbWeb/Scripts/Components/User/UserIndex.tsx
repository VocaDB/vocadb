import Layout from '@Components/Shared/Layout';
import { useSwitchboard } from '@Components/redial';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import ListUsersStore, {
	ListUsersRouteParams,
} from '@Stores/User/ListUsersStore';
import Ajv, { JSONSchemaType } from 'ajv';
import React from 'react';
import { useTranslation } from 'react-i18next';

import ListUsers from './Partials/ListUsers';

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);
const userRepo = new UserRepository(httpClient, urlMapper);
const listUsersStore = new ListUsersStore(userRepo);

// TODO: Use single Ajv instance. See https://ajv.js.org/guide/managing-schemas.html.
const ajv = new Ajv({ coerceTypes: true });

// TODO: Make sure that we compile schemas only once and re-use compiled validation functions. See https://ajv.js.org/guide/getting-started.html.
const schema: JSONSchemaType<ListUsersRouteParams> = require('@Stores/User/ListUsersRouteParams.schema.json');
const validate = ajv.compile(schema);

const UserIndex = (): React.ReactElement => {
	const { t } = useTranslation(['ViewRes']);

	useSwitchboard(validate, listUsersStore);

	return (
		<Layout title={t('ViewRes:Shared.Users')}>
			<ListUsers listUsersStore={listUsersStore} />
		</Layout>
	);
};

export default UserIndex;
