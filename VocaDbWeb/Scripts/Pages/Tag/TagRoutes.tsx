import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const TagIndex = React.lazy(() => import('./TagIndex'));
const TagEdit = React.lazy(() => import('./TagEdit'));
const TagMerge = React.lazy(() => import('./TagMerge'));
const TagVersions = React.lazy(() => import('./TagVersions'));
const TagViewVersion = React.lazy(() => import('./TagViewVersion'));
const TagDeleted = React.lazy(() => import('./TagDeleted'));

const TagRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<TagIndex />} />
			<Route path="Edit/:id" element={<TagEdit />} />
			<Route path="Merge/:id" element={<TagMerge />} />
			<Route path="Versions/:id" element={<TagVersions />} />
			<Route path="ViewVersion/:id" element={<TagViewVersion />} />
			<Route path="Deleted" element={<TagDeleted />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default TagRoutes;
