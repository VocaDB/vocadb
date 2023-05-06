import React from 'react';
import { Route, Routes } from 'react-router-dom';
import ErrorIndex from '../Error/ErrorIndex';

const HomeIndex = React.lazy(() => import('./HomeIndex'));

const HomeRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<HomeIndex />} />
			<Route path="*" element={<ErrorIndex />} />
		</Routes>
	);
};

export default HomeRoutes;
