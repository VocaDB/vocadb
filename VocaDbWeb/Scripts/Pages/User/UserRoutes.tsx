import { UserApiContract } from '@/DataContracts/User/UserApiContract';
import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import { userRepo } from '@/Repositories/UserRepository';
import { EntryUrlMapper } from '@/Shared/EntryUrlMapper';
import React from 'react';
import { Navigate, Route, Routes, useParams } from 'react-router-dom';

const UserIndex = React.lazy(() => import('./UserIndex'));
const UserEdit = React.lazy(() => import('./UserEdit'));
const UserEntryEdits = React.lazy(() => import('./UserEntryEdits'));
const UserFavoriteSongs = React.lazy(() => import('./UserFavoriteSongs'));
const UserMessages = React.lazy(() => import('./UserMessages'));
const UserMySettings = React.lazy(() => import('./UserMySettings'));
const UserRequestVerification = React.lazy(
	() => import('./UserRequestVerification'),
);

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
			<Route path="Edit/:id" element={<UserEdit />} />
			<Route path="EntryEdits/:id" element={<UserEntryEdits />} />
			<Route path="FavoriteSongs/:id" element={<UserFavoriteSongs />} />
			<Route path="Messages" element={<UserMessages />} />
			<Route path="MySettings" element={<UserMySettings />} />
			<Route path="RequestVerification" element={<UserRequestVerification />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default UserRoutes;
