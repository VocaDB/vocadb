import React from 'react';
import { Routes } from 'react-router';
import { Route } from 'react-router-dom';

const StatsIndex = React.lazy(() => import('./StatsIndex'));

const StatsRoutes = (): React.ReactElement => {
	return (
		<Routes>
			<Route path="/" element={<StatsIndex />} />
		</Routes>
	);
};

export default StatsRoutes;
