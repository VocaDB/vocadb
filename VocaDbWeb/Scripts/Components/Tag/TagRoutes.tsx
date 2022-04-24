import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const TagIndex = React.lazy(() => import('./TagIndex'));
const TagVersions = React.lazy(() => import('./TagVersions'));

const TagRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<TagIndex />} />
			<Route path="Versions/:id" element={<TagVersions />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default TagRoutes;
