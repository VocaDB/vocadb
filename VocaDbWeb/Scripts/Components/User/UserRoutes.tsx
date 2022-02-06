import ErrorNotFound from '@Components/Error/ErrorNotFound';
import UserApiContract from '@DataContracts/User/UserApiContract';
import UserRepository from '@Repositories/UserRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

const UserIndex = React.lazy(() => import('./UserIndex'));

const httpClient = new HttpClient();
const urlMapper = new UrlMapper(vdb.values.baseAddress);

const userRepo = new UserRepository(httpClient, urlMapper);

const UserDetailsNavigate = (): React.ReactElement => {
	const { id } = useParams();

	const [user, setUser] = React.useState<UserApiContract>();

	React.useEffect(() => {
		userRepo
			.getOne({ id: Number(id) })
			.then((user) => setUser(user))
			.catch((error) => {
				if (error.response) {
					if (error.response.status === 404)
						window.location.href = '/Error/NotFound';
				}

				throw error;
			});
	}, [id]);

	return user ? (
		<Navigate
			to={EntryUrlMapper.details_user_byName(user.name)}
			replace={true}
		/>
	) : (
		<></>
	);
};

const UserRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<UserIndex />} />
			<Route path="Details/:id" element={<UserDetailsNavigate />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default UserRoutes;
