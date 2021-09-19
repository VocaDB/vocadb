import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const UserIndex = React.lazy(() => import('./UserIndex'));

const UserRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<UserIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default UserRoutes;
