import ErrorNotFound from '@/Pages/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const HomeIndex = React.lazy(() => import('./HomeIndex'));

const HomeRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<HomeIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default HomeRoutes;
