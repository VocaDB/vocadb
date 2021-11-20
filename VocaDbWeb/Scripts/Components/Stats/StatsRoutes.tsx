import ErrorNotFound from '@Components/Error/ErrorNotFound';
import React from 'react';
import { Route, Routes } from 'react-router-dom';

const StatsIndex = React.lazy(() => import('./StatsIndex'));

const StatsRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="" element={<StatsIndex />} />
			<Route path="*" element={<ErrorNotFound />} />
		</Routes>
	);
};

export default StatsRoutes;
