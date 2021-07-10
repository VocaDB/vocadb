import React from 'react';
import { Route, Routes } from 'react-router-dom';

const UserIndex = React.lazy(() => import('./UserIndex'));

const UserRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="/" element={<UserIndex />} />
		</Routes>
	);
};

export default UserRoutes;
