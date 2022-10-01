import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const AdminIndex = React.lazy(() => import('./AdminIndex'));
const AdminManageEntryTagMappings = React.lazy(
	() => import('./AdminManageEntryTagMappings'),
);
const AdminManageTagMappings = React.lazy(
	() => import('./AdminManageTagMappings'),
);
const AdminManageWebhooks = React.lazy(() => import('./AdminManageWebhooks'));

const AdminRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<AdminIndex />} />
			<Route
				path="ManageEntryTagMappings"
				element={<AdminManageEntryTagMappings />}
			/>
			<Route path="ManageTagMappings" element={<AdminManageTagMappings />} />
			<Route path="ManageWebhooks" element={<AdminManageWebhooks />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default AdminRoutes;
