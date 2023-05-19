import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const AdminActiveEdits = React.lazy(() => import('./AdminActiveEdits'));
const AdminIndex = React.lazy(() => import('./AdminIndex'));
const AdminManageEntryTagMappings = React.lazy(
	() => import('./AdminManageEntryTagMappings'),
);
const AdminManageIPRules = React.lazy(
	() => import('../../Pages/Admin/AdminManageIPRules'),
);
const AdminManageTagMappings = React.lazy(
	() => import('./AdminManageTagMappings'),
);
const AdminManageWebhooks = React.lazy(() => import('./AdminManageWebhooks'));
const AdminViewAuditLog = React.lazy(() => import('./AdminViewAuditLog'));
const AdminViewEntryReports = React.lazy(
	() => import('./AdminManageEntryReports'),
);

const AdminRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<AdminIndex />} />
			<Route path="ActiveEdits" element={<AdminActiveEdits />} />
			<Route
				path="ManageEntryTagMappings"
				element={<AdminManageEntryTagMappings />}
			/>
			<Route path="ManageIPRules" element={<AdminManageIPRules />} />
			<Route path="ManageTagMappings" element={<AdminManageTagMappings />} />
			<Route path="ManageWebhooks" element={<AdminManageWebhooks />} />
			<Route path="ViewAuditLog" element={<AdminViewAuditLog />} />
			<Route path="ViewEntryReports" element={<AdminViewEntryReports />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default AdminRoutes;
