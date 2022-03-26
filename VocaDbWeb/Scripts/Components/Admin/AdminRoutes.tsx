import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const AdminIndex = React.lazy(() => import('./AdminIndex'));

const AdminRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<AdminIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default AdminRoutes;
